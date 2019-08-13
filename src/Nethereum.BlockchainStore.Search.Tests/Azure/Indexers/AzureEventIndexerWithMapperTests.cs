using Microsoft.Azure.Search.Models;
using Nethereum.BlockchainStore.Search.Azure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SearchDocument = Nethereum.BlockchainStore.Search.Tests.TestData.IndexerTestData.SearchDocument;
using TransferEvent = Nethereum.BlockchainStore.Search.Tests.TestData.Contracts.StandardContract.TransferEvent;

namespace Nethereum.BlockchainStore.Search.Tests.Azure
{
    public class AzureEventIndexerWithMapperTests
    {
        [Fact]
        public async Task MapsEventDtoToSearchDocument()
        {
            var index = new Index(); //for proper use, this index should have been prepopulated
            var mockSearchIndexClient = new SearchIndexClientMock<SearchDocument>();

            var indexer = new AzureEventIndexer<TransferEvent, SearchDocument>(
                mockSearchIndexClient.SearchIndexClient, (tfr) => new SearchDocument(tfr.Log.TransactionHash, tfr.Log.LogIndex));

            var eventLog = TestData.Contracts.StandardContract.SampleTransferEventLog();

            await indexer.IndexAsync(eventLog);

            Assert.Single(mockSearchIndexClient.IndexedBatches);
            var firstIndexAction = mockSearchIndexClient.IndexedBatches[0].Actions.First();
            Assert.Equal(eventLog.Log.TransactionHash, firstIndexAction.Document.TransactionHash);
            Assert.Equal(eventLog.Log.LogIndex.Value.ToString(), firstIndexAction.Document.LogIndex);
        } 
    }
}
