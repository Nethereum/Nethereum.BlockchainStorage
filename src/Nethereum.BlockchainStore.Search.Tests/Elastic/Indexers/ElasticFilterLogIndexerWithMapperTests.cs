using Nethereum.BlockchainStore.Search.ElasticSearch;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SearchDocument = Nethereum.BlockchainStore.Search.Tests.TestData.IndexerTestData.SearchDocument;

namespace Nethereum.BlockchainStore.Search.Tests.Elastic
{
    public class ElasticFilterLogIndexerWithMapperTests
    {
        [Fact]
        public async Task MapsFilterLogToSearchDocument()
        {
            var mockElasticClient = new MockElasticClient();

            var indexer = new ElasticFilterLogIndexer<SearchDocument>("my-index",
                mockElasticClient.ElasticClient, (tfr) => new SearchDocument(tfr.TransactionHash, tfr.LogIndex));

            var log = TestData.Contracts.StandardContract.SampleTransferLog();

            await indexer.IndexAsync(log);

            Assert.Single(mockElasticClient.BulkRequests);
            var searchDoc = mockElasticClient.GetFirstBulkOperation().GetBody() as SearchDocument;
            Assert.Equal(log.TransactionHash, searchDoc.TransactionHash);
            Assert.Equal(log.LogIndex.Value.ToString(), searchDoc.LogIndex);
        }
    }
}
