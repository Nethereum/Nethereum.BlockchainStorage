using Microsoft.Azure.Search.Models;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Hex.HexTypes;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using TransferEvent = Nethereum.BlockchainStore.Search.Tests.TestData.Contracts.StandardContract.TransferEvent;

namespace Nethereum.BlockchainStore.Search.Tests.Azure
{
    public class AzureEventIndexerWithMapperTests
    {
        public class SearchDocument
        {
            public SearchDocument(string transactionHash, HexBigInteger logIndex)
            {
                TransactionHash = transactionHash;
                LogIndex = logIndex.Value.ToString();
            }

            public string TransactionHash { get; }
            public string LogIndex { get; }
        }

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
