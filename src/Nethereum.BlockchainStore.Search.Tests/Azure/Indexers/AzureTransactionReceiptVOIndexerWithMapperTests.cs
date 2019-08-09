using Microsoft.Azure.Search.Models;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests.Azure
{
    public class AzureTransactionReceiptVOIndexerWithMapperTests
    {
        public class SearchDocument
        {
            public SearchDocument(TransactionReceiptVO transactionReceiptVO)
            {
                TransactionReceiptVO = transactionReceiptVO;
            }

            public TransactionReceiptVO TransactionReceiptVO { get; }
        }

        [Fact]
        public async Task MapsTransactionToSearchDocument()
        {
            var index = new Index();
            var mockSearchIndexClient = new SearchIndexClientMock<SearchDocument>();

            var indexer = new AzureTransactionReceiptVOIndexer<SearchDocument>(
                index, mockSearchIndexClient.SearchIndexClient, (tx) => new SearchDocument(tx));

            TransactionReceiptVO transaction = CreateSampleTransaction();

            await indexer.IndexAsync(transaction);

            Assert.Single(mockSearchIndexClient.IndexedBatches);
            var firstIndexAction = mockSearchIndexClient.IndexedBatches[0].Actions.First();
            var document = firstIndexAction.Document;
            //check mapping
            Assert.Same(transaction, document.TransactionReceiptVO);

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
