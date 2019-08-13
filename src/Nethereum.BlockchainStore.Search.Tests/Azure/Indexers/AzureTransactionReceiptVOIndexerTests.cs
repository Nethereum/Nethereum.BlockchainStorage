using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.BlockchainStore.Search.Tests.TestData;
using Nethereum.RPC.Eth.DTOs;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests.Azure
{
    public class AzureTransactionReceiptVOIndexerTests
    {
        [Fact]
        public async Task MapsTransactionToGenericSearchDocument()
        {
            var indexDefinition = new TransactionReceiptVOIndexDefinition("my-transactions");
            var index = indexDefinition.ToAzureIndex();
            var mockSearchIndexClient = new SearchIndexClientMock<GenericSearchDocument>();

            var indexer = new AzureTransactionReceiptVOIndexer(
                mockSearchIndexClient.SearchIndexClient, indexDefinition);

            TransactionReceiptVO transaction = IndexerTestData.CreateSampleTransaction();

            await indexer.IndexAsync(transaction);

            Assert.Single(mockSearchIndexClient.IndexedBatches);
            var firstIndexAction = mockSearchIndexClient.IndexedBatches[0].Actions.First();
            var document = firstIndexAction.Document;
            //check generic mapping
            Assert.Equal(transaction.Transaction.BlockNumber.ToString(), document[PresetSearchFieldName.tx_block_number.ToString()]);

        }
    }
}
