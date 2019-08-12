using Microsoft.Azure.Search.Models;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Hex.HexTypes;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using TransferEvent = Nethereum.BlockchainStore.Search.Tests.TestData.Contracts.StandardContract.TransferEvent;

namespace Nethereum.BlockchainStore.Search.Tests.Azure
{
    public class AzureIndexerTests
    {
        public class Source
        {
        }

        public class SearchDocument
        {
        }

        [Fact]
        public async Task MapsSourceToSearchDocument()
        {
            var index = new Index(); //for proper use, this index should have been prepopulated
            var mockSearchIndexClient = new SearchIndexClientMock<SearchDocument>();
            var mappedSearchDocument = new SearchDocument();

            var indexer = new AzureIndexer<Source, SearchDocument>(
                mockSearchIndexClient.SearchIndexClient, (tfr) => mappedSearchDocument);

            var source = new Source();

            await indexer.IndexAsync(source);

            Assert.Single(mockSearchIndexClient.IndexedBatches);
            var firstIndexAction = mockSearchIndexClient.IndexedBatches[0].Actions.First();
            Assert.Same(mappedSearchDocument, firstIndexAction.Document);
        } 
    }
}
