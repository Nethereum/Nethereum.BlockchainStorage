using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.BlockchainStore.Search.Tests.TestData;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Nethereum.BlockchainStore.Search.Tests.TestData.Contracts.StandardContract;

namespace Nethereum.BlockchainStore.Search.Tests.Azure
{
    public class AzureFunctionIndexerTests
    {
        [Fact]
        public async Task MapsFunctionDtoToGenericSearchDocument()
        {
            var indexDefinition = new FunctionIndexDefinition<TransferFunction>();
            var index = indexDefinition.ToAzureIndex();
            var mockSearchIndexClient = new SearchIndexClientMock<GenericSearchDocument>();

            var indexer = new AzureFunctionIndexer<TransferFunction>(
                mockSearchIndexClient.SearchIndexClient, indexDefinition);

            TransactionForFunctionVO<TransferFunction> transactionForFunction = IndexerTestData.CreateSampleTransactionForFunction();

            await indexer.IndexAsync(transactionForFunction);

            Assert.Single(mockSearchIndexClient.IndexedBatches);
            var firstIndexAction = mockSearchIndexClient.IndexedBatches[0].Actions.First();
            var document = firstIndexAction.Document;
            //check generic mapping
            Assert.Equal(transactionForFunction.Transaction.BlockNumber.ToString(), document[PresetSearchFieldName.tx_block_number.ToString()]);
            //check function message mapping
            Assert.Equal(transactionForFunction.FunctionMessage.To, document["to"]);
            Assert.Equal(transactionForFunction.FunctionMessage.Value.ToString(), document["value"]);

        }
    }
}
