using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests.Azure
{
    public class AzureTransactionReceiptVOIndexerTests
    {
        [Fact]
        public async Task MapsTransactionToDictionary()
        {
            var indexDefinition = new TransactionReceiptVOIndexDefinition("my-transactions");
            var index = indexDefinition.ToAzureIndex();
            var mockSearchIndexClient = new SearchIndexClientMock<Dictionary<string, object>>();

            var indexer = new AzureTransactionReceiptVOIndexer(
                mockSearchIndexClient.SearchIndexClient, indexDefinition);

            TransactionReceiptVO transaction = CreateSampleTransaction();

            await indexer.IndexAsync(transaction);

            Assert.Single(mockSearchIndexClient.IndexedBatches);
            var firstIndexAction = mockSearchIndexClient.IndexedBatches[0].Actions.First();
            var document = firstIndexAction.Document;
            //check generic mapping
            Assert.Equal(transaction.Transaction.BlockNumber.ToString(), document[PresetSearchFieldName.tx_block_number.ToString()]);

        }

        private static TransactionReceiptVO CreateSampleTransaction()
        {
            Block block = new Block { Number = new HexBigInteger(new BigInteger(10)) };
            var tx = new Transaction
            {
                BlockNumber = block.Number,
                TransactionHash = "0x19ce02e0b4fdf5cfee0ed21141b38c2d88113c58828c771e813ce2624af127cd",
                TransactionIndex = new HexBigInteger(new BigInteger(0))
            };

            var receipt = new TransactionReceipt { };
            return new TransactionReceiptVO(block, tx, receipt, false);
        }
    }
}
