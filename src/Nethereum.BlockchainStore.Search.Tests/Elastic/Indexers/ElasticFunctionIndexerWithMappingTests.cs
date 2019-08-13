using Nethereum.BlockchainStore.Search.ElasticSearch;
using Nethereum.BlockchainStore.Search.Tests.TestData;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Nethereum.BlockchainStore.Search.Tests.TestData.Contracts.StandardContract;
using SearchDocument = Nethereum.BlockchainStore.Search.Tests.TestData.IndexerTestData.SearchDocument;

namespace Nethereum.BlockchainStore.Search.Tests.Elastic
{
    public class ElasticFunctionIndexerWithMappingTests
    {
        [Fact]
        public async Task MapsFunctionDtoToSearchDocument()
        {
            var mockElasticClient = new MockElasticClient();

            var indexer = new ElasticFunctionIndexer<TransferFunction, SearchDocument>("my-index",
                mockElasticClient.ElasticClient, (tx) => new SearchDocument(tx));

            TransactionForFunctionVO<TransferFunction> transactionForFunction = IndexerTestData.CreateSampleTransactionForFunction();

            await indexer.IndexAsync(transactionForFunction);

            Assert.Single(mockElasticClient.BulkRequests);
            var indexedDoc = mockElasticClient.GetFirstBulkOperation().GetBody() as SearchDocument;

            //check function message mapping
            Assert.Equal(transactionForFunction.Transaction.TransactionHash, indexedDoc.TransactionHash);
            Assert.Equal(transactionForFunction.FunctionMessage.To, indexedDoc.To);
            Assert.Equal(transactionForFunction.FunctionMessage.Value.ToString(), indexedDoc.Value);
        }

    }
}
