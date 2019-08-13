using Microsoft.Azure.Search.Models;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Hex.HexTypes;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SearchDocument = Nethereum.BlockchainStore.Search.Tests.TestData.IndexerTestData.SearchDocument;

namespace Nethereum.BlockchainStore.Search.Tests.Azure
{
    public class AzureFilterLogIndexerWithMapperTests
    {

        [Fact]
        public async Task MapsFilterLogToSearchDocument()
        {
            var index = new Index(); //for proper use, this index should have been prepopulated
            var mockSearchIndexClient = new SearchIndexClientMock<SearchDocument>();

            var indexer = new AzureFilterLogIndexer<SearchDocument>(
                mockSearchIndexClient.SearchIndexClient, (tfr) => new SearchDocument(tfr.TransactionHash, tfr.LogIndex));

            var log = TestData.Contracts.StandardContract.SampleTransferLog();

            await indexer.IndexAsync(log);

            Assert.Single(mockSearchIndexClient.IndexedBatches);
            var firstIndexAction = mockSearchIndexClient.IndexedBatches[0].Actions.First();
            Assert.Equal(log.TransactionHash, firstIndexAction.Document.TransactionHash);
            Assert.Equal(log.LogIndex.Value.ToString(), firstIndexAction.Document.LogIndex);
        } 
    }
}
