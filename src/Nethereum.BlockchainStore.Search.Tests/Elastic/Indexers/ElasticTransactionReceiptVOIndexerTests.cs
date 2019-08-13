using Nethereum.BlockchainStore.Search.Elastic;
using Nethereum.BlockchainStore.Search.Tests.TestData;
using Nethereum.RPC.Eth.DTOs;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests.Elastic
{
    public class ElasticTransactionReceiptVOIndexerTests
    {
        [Fact]
        public async Task MapsTransactionToGenericSearchDocument()
        {
            var indexDefinition = new TransactionReceiptVOIndexDefinition("my-transactions");
            var mockElasticClient = new MockElasticClient();

            var indexer = new ElasticTransactionReceiptVOIndexer("my-index",
                mockElasticClient.ElasticClient, indexDefinition);

            TransactionReceiptVO transaction = IndexerTestData.CreateSampleTransaction();

            await indexer.IndexAsync(transaction);

            Assert.Single(mockElasticClient.BulkRequests);
            var indexedDoc = mockElasticClient.GetFirstBulkOperation().GetBody() as GenericSearchDocument;
            //check generic mapping
            Assert.Equal(transaction.Transaction.BlockNumber.ToString(), indexedDoc[PresetSearchFieldName.tx_block_number.ToString()]);

        }
    }
}
