using Nethereum.BlockchainStore.Search.ElasticSearch;
using Nethereum.BlockchainStore.Search.Tests.TestData;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Nethereum.BlockchainStore.Search.Tests.TestData.Contracts.StandardContract;

namespace Nethereum.BlockchainStore.Search.Tests.Elastic
{
    public class ElasticFunctionIndexerTests
    {
        [Fact]
        public async Task MapsFunctionDtoToGenericSearchDocument()
        {
            var indexDefinition = new FunctionIndexDefinition<TransferFunction>();
            var mockSearchIndexClient = new MockElasticClient();

            var indexer = new ElasticFunctionIndexer<TransferFunction>("transfer-functions", 
                mockSearchIndexClient.ElasticClient, indexDefinition);

            TransactionForFunctionVO<TransferFunction> transactionForFunction = IndexerTestData.CreateSampleTransactionForFunction();

            await indexer.IndexAsync(transactionForFunction);

            Assert.Single(mockSearchIndexClient.BulkRequests);
            var indexedDoc = mockSearchIndexClient.GetFirstBulkOperation().GetBody() as GenericSearchDocument;

            //check generic mapping
            Assert.Equal(transactionForFunction.Transaction.BlockNumber.ToString(), indexedDoc[PresetSearchFieldName.tx_block_number.ToString()]);
            //check function message mapping
            Assert.Equal(transactionForFunction.FunctionMessage.To, indexedDoc["to"]);
            Assert.Equal(transactionForFunction.FunctionMessage.Value.ToString(), indexedDoc["value"]);

        }
    }
}
