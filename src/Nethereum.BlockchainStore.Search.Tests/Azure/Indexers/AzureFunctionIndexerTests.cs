using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

            TransactionForFunctionVO<TransferFunction> transactionForFunction = CreateSampleTransactionForFunction();

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

        private static TransactionForFunctionVO<TransferFunction> CreateSampleTransactionForFunction()
        {
            Block block = new Block { Number = new HexBigInteger(new BigInteger(10)) };
            var tx = new Transaction
            {
                BlockNumber = block.Number,
                TransactionHash = "0x19ce02e0b4fdf5cfee0ed21141b38c2d88113c58828c771e813ce2624af127cd",
                TransactionIndex = new HexBigInteger(new BigInteger(0))
            };

            var receipt = new TransactionReceipt { };
            var txWithReceipt = new TransactionReceiptVO(block, tx, receipt, false);
            var functionMessage = new TransferFunction
            {
                FromAddress = "0x12890d2cce102216644c59dae5baed380d84830c",
                To = "0x22890d2cce102216644c59dae5baed380d84830c",
                Value = new BigInteger(101)
            };

            return new TransactionForFunctionVO<TransferFunction>(txWithReceipt, functionMessage);
        }
    }
}
