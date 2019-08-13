using Nethereum.BlockchainStore.Search.Elastic;
using Nethereum.BlockchainStore.Search.Tests.TestData;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using Xunit;
using SearchDocument = Nethereum.BlockchainStore.Search.Tests.TestData.IndexerTestData.SearchDocument;

namespace Nethereum.BlockchainStore.Search.Tests.Elastic
{
    public class ElasticTransactionReceiptVOIndexerWithMapperTests
    {
        [Fact]
        public async Task MapsTransactionToSearchDocument()
        {
            var mockElasticClient = new MockElasticClient();

            var indexer = new ElasticTransactionReceiptVOIndexer<SearchDocument>("my-index",
                mockElasticClient.ElasticClient, (tx) => new SearchDocument(tx));

            TransactionReceiptVO transaction = IndexerTestData.CreateSampleTransaction();

            await indexer.IndexAsync(transaction);

            Assert.Single(mockElasticClient.BulkRequests);
            var indexedDocument = mockElasticClient.GetFirstBulkOperation().GetBody() as SearchDocument;
            //check mapping
            Assert.Same(transaction, indexedDocument.TransactionReceiptVO);

        }
    }
}
