using Moq;
using Nethereum.BlockchainStore.Processing;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Processing;
using Xunit;
using IBlockFilter = Nethereum.BlockchainProcessing.Processors.IBlockFilter;

namespace Nethereum.BlockchainStore.Tests.Processing.BlockProcessorTests
{
    public class ProcessBlockAsyncTests
    {
        public class WhenBlockIsNull : BlockProcessorTestBase
        {
            [Fact]
            public async Task ThrowsBlockNotFoundException()
            {
                //setup
                MockBlockProxy
                    .Setup(p => p.GetBlockWithTransactionsAsync(BlockNumber))
                    .ReturnsAsync((BlockWithTransactions) null);

                //execute
                await Assert.ThrowsAsync<BlockNotFoundException>(
                    async () => await BlockProcessor.ProcessBlockAsync(BlockNumber));
            }
        }

        public class WhenBlockIsNotNull : BlockProcessorTestBase
        {
            const string TxHash1 = "0xc185cc7b9f7862255b82fd41be561fdc94d030567d0b41292008095bf31c39b9";
            const string TxHash2 = "0x8cb49adf5d0db2f85b092bb39366f108a68d29fffa177d172f838ba551842fd3";
            private readonly BlockWithTransactions _stubBlock;

            public WhenBlockIsNotNull()
            {
                _stubBlock = new BlockWithTransactions
                {
                    Number = new HexBigInteger(BlockNumber),
                    Transactions = new[]
                    {
                        new Transaction{TransactionHash = TxHash1},
                        new Transaction{TransactionHash = TxHash2}
                    }
                };

                MockBlockProxy
                    .Setup(p => p.GetBlockWithTransactionsAsync(BlockNumber))
                    .ReturnsAsync(_stubBlock);
            }

            [Fact]
            public async Task Invokes_BlockHandler_And_TransactionProcessor()
            {
                //execute
                await BlockProcessor.ProcessBlockAsync(BlockNumber);

                //assert
                MockBlockHandler.Verify(b => b.HandleAsync(_stubBlock), Times.Once);

                foreach (var txn in _stubBlock.Transactions)
                {
                    MockTransactionProcessor
                        .Verify(t => t.ProcessTransactionAsync(_stubBlock, txn), Times.Once);
                }

            }

            [Fact]
            public async Task Processes_Blocks_Which_Match_Filter()
            {
                var matchingBlockFilter = new Mock<IBlockFilter>();
                matchingBlockFilter.Setup(b => b.IsMatchAsync(_stubBlock)).ReturnsAsync(true);
                BlockFilters.Add(matchingBlockFilter.Object);

                //execute
                await BlockProcessor.ProcessBlockAsync(BlockNumber);

                //assert
                matchingBlockFilter.Verify(b => b.IsMatchAsync(_stubBlock), Times.Once);

                MockBlockHandler.Verify(b => b.HandleAsync(_stubBlock), Times.Once);

                foreach (var txn in _stubBlock.Transactions)
                {
                    MockTransactionProcessor
                        .Verify(t => t.ProcessTransactionAsync( _stubBlock, txn), Times.Once);
                }

            }

            [Fact]
            public async Task When_Filter_Does_Not_Match_Ignores_Block()
            {
                var nonMatchingFilter = new Moq.Mock<IBlockFilter>();
                nonMatchingFilter.Setup(b => b.IsMatchAsync(_stubBlock)).ReturnsAsync(false);
                BlockFilters.Add(nonMatchingFilter.Object);

                //execute
                await BlockProcessor.ProcessBlockAsync(BlockNumber);

                //assert
                nonMatchingFilter.Verify(b => b.IsMatchAsync(_stubBlock), Times.Once);

                MockBlockHandler.Verify(b => b.HandleAsync(_stubBlock), Times.Never);

                MockTransactionProcessor
                    .Verify(t => t.ProcessTransactionAsync(
                        It.IsAny<Block>(), It.IsAny<Transaction>()), Times.Never);

            }
        }
    }
}
