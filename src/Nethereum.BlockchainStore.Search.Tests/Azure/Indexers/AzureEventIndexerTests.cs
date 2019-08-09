using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using TransferEvent = Nethereum.BlockchainStore.Search.Tests.TestData.Contracts.StandardContract.TransferEvent;

namespace Nethereum.BlockchainStore.Search.Tests.Azure
{
    public class AzureEventIndexerTests
    {
        [Fact]
        public async Task MapsEventDtoToDictionary()
        {
            var indexDefinition = new EventIndexDefinition<TransferEvent>();
            var index = indexDefinition.ToAzureIndex();
            var mockSearchIndexClient = new SearchIndexClientMock<Dictionary<string, object>>();

            var indexer = new AzureEventIndexer<TransferEvent>(
                index, mockSearchIndexClient.SearchIndexClient, indexDefinition);

            var eventLog = TestData.Contracts.StandardContract.SampleTransferEventLog();

            await indexer.IndexAsync(eventLog);

            Assert.Single(mockSearchIndexClient.IndexedBatches);
            var firstIndexAction = mockSearchIndexClient.IndexedBatches[0].Actions.First();
            Assert.Equal(eventLog.Log.Key(), firstIndexAction.Document[PresetSearchFieldName.log_key.ToString()]);
        } 
    }
}
