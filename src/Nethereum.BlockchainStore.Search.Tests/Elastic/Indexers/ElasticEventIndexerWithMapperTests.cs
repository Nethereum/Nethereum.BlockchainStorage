using Nethereum.BlockchainStore.Search.ElasticSearch;
using Nethereum.BlockchainStore.Search.Tests.TestData;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using TransferEvent = Nethereum.BlockchainStore.Search.Tests.TestData.Contracts.StandardContract.TransferEvent;
using SearchDocument = Nethereum.BlockchainStore.Search.Tests.TestData.IndexerTestData.SearchDocument;

namespace Nethereum.BlockchainStore.Search.Tests.Elastic
{
    public class ElasticEventIndexerWithMapperTests
    {

        [Fact]
        public async Task MapsEventDtoToSearchDocument()
        {
            var mockSearchIndexClient = new MockElasticClient();

            var indexer = new ElasticEventIndexer<TransferEvent, SearchDocument>("transfers",
                mockSearchIndexClient.ElasticClient, (tfr) => new SearchDocument(tfr.Log.TransactionHash, tfr.Log.LogIndex));

            var eventLog = TestData.Contracts.StandardContract.SampleTransferEventLog();

            await indexer.IndexAsync(eventLog);

            Assert.Single(mockSearchIndexClient.BulkRequests);
            var actualSearchDocument = mockSearchIndexClient.GetFirstBulkOperation().GetBody() as SearchDocument;
            Assert.Equal(eventLog.Log.TransactionHash, actualSearchDocument.TransactionHash);
            Assert.Equal(eventLog.Log.LogIndex.Value.ToString(), actualSearchDocument.LogIndex);
        }
    }
}
