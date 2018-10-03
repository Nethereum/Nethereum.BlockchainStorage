using Moq;
using Nethereum.BlockchainStore.Processors;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Processing;
using Xunit;

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
                    .Setup(p => p.GetBlockWithTransactionsHashesByNumber(BlockNumber))
                    .ReturnsAsync((BlockWithTransactionHashes) null);

                //execute
                await Assert.ThrowsAsync<BlockNotFoundException>(
                    async () => await BlockProcessor.ProcessBlockAsync(BlockNumber));
            }
        }

        public class WhenBlockIsNotNull : BlockProcessorTestBase
        {
            const string TxHash1 = "0xc185cc7b9f7862255b82fd41be561fdc94d030567d0b41292008095bf31c39b9";
            const string TxHash2 = "0x8cb49adf5d0db2f85b092bb39366f108a68d29fffa177d172f838ba551842fd3";
            private readonly BlockWithTransactionHashes _stubBlock;

            public WhenBlockIsNotNull()
            {
                _stubBlock = new BlockWithTransactionHashes
                {
                    Number = new HexBigInteger(BlockNumber),
                    TransactionHashes = new[] { TxHash1, TxHash2 }
                };

                MockBlockProxy
                    .Setup(p => p.GetBlockWithTransactionsHashesByNumber(BlockNumber))
                    .ReturnsAsync(_stubBlock);
            }

            [Fact]
            public async Task PersistsBlockAndCallsTransactionProcessor()
            {
                //execute
                await BlockProcessor.ProcessBlockAsync(BlockNumber);

                //assert
                MockBlockRepository.Verify(b => b.UpsertBlockAsync(_stubBlock), Times.Once);

                foreach (var txHash in _stubBlock.TransactionHashes)
                {
                    MockTransactionProcessor
                        .Verify(t => t.ProcessTransactionAsync(txHash, _stubBlock), Times.Once);
                }

            }

            [Fact]
            public async Task ProcessesBlocksWhichMatchFilter()
            {
                var mockBlockFilter = new Mock<IBlockFilter>();
                mockBlockFilter.Setup(b => b.IsMatchAsync(_stubBlock)).ReturnsAsync(true);
                BlockFilters.Add(mockBlockFilter.Object);

                //execute
                await BlockProcessor.ProcessBlockAsync(BlockNumber);

                //assert
                mockBlockFilter.Verify(b => b.IsMatchAsync(_stubBlock), Times.Once);

                MockBlockRepository.Verify(b => b.UpsertBlockAsync(_stubBlock), Times.Once);

                foreach (var txHash in _stubBlock.TransactionHashes)
                {
                    MockTransactionProcessor
                        .Verify(t => t.ProcessTransactionAsync(txHash, _stubBlock), Times.Once);
                }

            }

            [Fact]
            public async Task IgnoresBlocksWhereFilterDoesNotMatch()
            {
                var blockNumberGreaterThan10Filter = new BlockFilter(
                    b => Task.FromResult(b.Number.Value > 10));

                BlockFilters.Add(blockNumberGreaterThan10Filter);

                //execute
                await BlockProcessor.ProcessBlockAsync(BlockNumber);

                //assert
                MockBlockRepository.Verify(b => b.UpsertBlockAsync(_stubBlock), Times.Never);

                MockTransactionProcessor
                    .Verify(t => t.ProcessTransactionAsync(
                        It.IsAny<String>(), It.IsAny<BlockWithTransactionHashes>()), Times.Never);

            }
        }
    }
}
