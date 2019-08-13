using Nethereum.BlockchainStore.Search.Elastic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SearchDocument = Nethereum.BlockchainStore.Search.Tests.TestData.IndexerTestData.SearchDocument;

namespace Nethereum.BlockchainStore.Search.Tests.Elastic
{
    public class ElasticIndexerTests
    {
        public class Source{}

        [Fact]
        public async Task MapsSourceToSearchDocument()
        {
            var mockElasticClient = new MockElasticClient();
            var mappedSearchDocument = new SearchDocument();

            var indexer = new ElasticIndexer<Source, SearchDocument>("my-index",
                mockElasticClient.ElasticClient, (tfr) => mappedSearchDocument);

            var source = new Source();

            await indexer.IndexAsync(source);

            Assert.Single(mockElasticClient.BulkRequests);
            var indexedDoc = mockElasticClient.GetFirstBulkOperation().GetBody() as SearchDocument;
            Assert.Same(mappedSearchDocument, indexedDoc);
        }
    }
}
