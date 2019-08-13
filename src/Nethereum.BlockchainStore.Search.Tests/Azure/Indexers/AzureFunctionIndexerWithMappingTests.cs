using Microsoft.Azure.Search.Models;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.BlockchainStore.Search.Tests.TestData;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Nethereum.BlockchainStore.Search.Tests.TestData.Contracts.StandardContract;
using SearchDocument = Nethereum.BlockchainStore.Search.Tests.TestData.IndexerTestData.SearchDocument;

namespace Nethereum.BlockchainStore.Search.Tests.Azure
{
    public class AzureFunctionIndexerWithMappingTests
    {
        [Fact]
        public async Task MapsFunctionDtoToSearchDocument()
        {
            var index = new Index();
            var mockSearchIndexClient = new SearchIndexClientMock<SearchDocument>();

            var indexer = new AzureFunctionIndexer<TransferFunction, SearchDocument>(
                mockSearchIndexClient.SearchIndexClient, (tx) => new SearchDocument(tx));

            TransactionForFunctionVO<TransferFunction> transactionForFunction = IndexerTestData.CreateSampleTransactionForFunction();

            await indexer.IndexAsync(transactionForFunction);

            Assert.Single(mockSearchIndexClient.IndexedBatches);
            var firstIndexAction = mockSearchIndexClient.IndexedBatches[0].Actions.First();
            var document = firstIndexAction.Document;

            //check function message mapping
            Assert.Equal(transactionForFunction.Transaction.TransactionHash, document.TransactionHash);
            Assert.Equal(transactionForFunction.FunctionMessage.To, document.To);
            Assert.Equal(transactionForFunction.FunctionMessage.Value.ToString(), document.Value);
        }

    }
}
