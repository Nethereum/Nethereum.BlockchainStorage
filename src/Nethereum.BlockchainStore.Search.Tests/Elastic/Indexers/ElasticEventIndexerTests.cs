using Nethereum.BlockchainStore.Search.ElasticSearch;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using TransferEvent = Nethereum.BlockchainStore.Search.Tests.TestData.Contracts.StandardContract.TransferEvent;

namespace Nethereum.BlockchainStore.Search.Tests.Elastic
{

    public class ElasticEventIndexerTests
    {
        [Fact]
        public async Task MapsEventDtoToGenericSearchDocument()
        {
            var indexDefinition = new EventIndexDefinition<TransferEvent>();
            var mockElasticClient = new MockElasticClient();

            var indexer = new ElasticEventIndexer<TransferEvent>(
                "Transfers", mockElasticClient.ElasticClient, indexDefinition);

            var eventLog = TestData.Contracts.StandardContract.SampleTransferEventLog();

            await indexer.IndexAsync(eventLog);

            Assert.Single(mockElasticClient.BulkRequests);
            var actualOperation = mockElasticClient.GetFirstBulkOperation();

            Assert.NotNull(actualOperation);
            Assert.Equal("index", actualOperation.Operation);
            Assert.Equal(typeof(GenericSearchDocument), actualOperation.ClrType);

        } 
    }
}
