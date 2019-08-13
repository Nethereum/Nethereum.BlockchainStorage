using Microsoft.Azure.Search.Models;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.BlockchainStore.Search.Tests.TestData;
using Nethereum.RPC.Eth.DTOs;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SearchDocument = Nethereum.BlockchainStore.Search.Tests.TestData.IndexerTestData.SearchDocument;

namespace Nethereum.BlockchainStore.Search.Tests.Azure
{
    public class AzureTransactionReceiptVOIndexerWithMapperTests
    {
        [Fact]
        public async Task MapsTransactionToSearchDocument()
        {
            var index = new Index();
            var mockSearchIndexClient = new SearchIndexClientMock<SearchDocument>();

            var indexer = new AzureTransactionReceiptVOIndexer<SearchDocument>(
                mockSearchIndexClient.SearchIndexClient, (tx) => new SearchDocument(tx));

            TransactionReceiptVO transaction = IndexerTestData.CreateSampleTransaction();

            await indexer.IndexAsync(transaction);

            Assert.Single(mockSearchIndexClient.IndexedBatches);
            var firstIndexAction = mockSearchIndexClient.IndexedBatches[0].Actions.First();
            var document = firstIndexAction.Document;
            //check mapping
            Assert.Same(transaction, document.TransactionReceiptVO);

        }
    }
}
