using Moq;
using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.Web3Abstractions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Processing;
using Xunit;

namespace Nethereum.BlockchainStore.Tests.Processing
{
    public class BlockProcessorTests
    {
        private readonly Mock<IGetBlockWithTransactionHashesByNumber> _mockBlockProxy;
        private readonly Mock<IBlockRepository> _mockBlockRepository;
        private readonly Mock<ITransactionProcessor> _mockTransactionProcessor;
        private readonly List<IBlockFilter> _blockFilters;

        public BlockProcessorTests()
        {
            _mockBlockProxy = new Mock<IGetBlockWithTransactionHashesByNumber>();
            _mockBlockRepository = new Mock<IBlockRepository>();
            _mockTransactionProcessor = new Mock<ITransactionProcessor>();
            _blockFilters = new List<IBlockFilter>();               
        }

        [Fact]
        public async Task ProcessBlock_PersistsBlockAndCallsTransactionProcessor()
        {
            //setup
            const long blockNumber = 9;

            BlockProcessor blockProcessor = CreateBlockProcessor();

            const string txHash1 = "0xc185cc7b9f7862255b82fd41be561fdc94d030567d0b41292008095bf31c39b9";
            const string txHash2 = "0x8cb49adf5d0db2f85b092bb39366f108a68d29fffa177d172f838ba551842fd3";

            var stubBlock = new BlockWithTransactionHashes
            {
                Number = new HexBigInteger(blockNumber),
                TransactionHashes = new string[]{txHash1, txHash2}
            };

            _mockBlockProxy
                .Setup(p => p.GetBlockWithTransactionsHashesByNumber(9))
                .ReturnsAsync(stubBlock);

            //execute
            await blockProcessor.ProcessBlockAsync(blockNumber);

            //assert
            _mockBlockRepository.Verify(b => b.UpsertBlockAsync(stubBlock), Times.Once);

            foreach (var txHash in stubBlock.TransactionHashes)
            {
                _mockTransactionProcessor
                    .Verify(t => t.ProcessTransactionAsync(txHash, stubBlock), Times.Once);
            }
  
        }

        [Fact]
        public async Task ProcessBlock_ProcessesBlocksWhichMatchFilter()
        {
            //setup
            const long blockNumber = 9;

            const string txHash1 = "0xc185cc7b9f7862255b82fd41be561fdc94d030567d0b41292008095bf31c39b9";
            const string txHash2 = "0x8cb49adf5d0db2f85b092bb39366f108a68d29fffa177d172f838ba551842fd3";

            var stubBlock = new BlockWithTransactionHashes
            {
                Number = new HexBigInteger(blockNumber),
                TransactionHashes = new []{txHash1, txHash2}
            };

            
            var mockBlockFilter = new Mock<IBlockFilter>();
            mockBlockFilter.Setup(b => b.IsMatchAsync(stubBlock)).ReturnsAsync(true);
            _blockFilters.Add(mockBlockFilter.Object);

            var blockProcessor = CreateBlockProcessor();

            _mockBlockProxy
                .Setup(p => p.GetBlockWithTransactionsHashesByNumber(9))
                .ReturnsAsync(stubBlock);

            //execute
            await blockProcessor.ProcessBlockAsync(blockNumber);

            //assert
            mockBlockFilter.Verify(b => b.IsMatchAsync(stubBlock), Times.Once);

            _mockBlockRepository.Verify(b => b.UpsertBlockAsync(stubBlock), Times.Once);

            foreach (var txHash in stubBlock.TransactionHashes)
            {
                _mockTransactionProcessor
                    .Verify(t => t.ProcessTransactionAsync(txHash, stubBlock), Times.Once);
            }
  
        }

        [Fact]
        public async Task ProcessBlock_IgnoresBlocksWhereFilterDoesNotMatch()
        {
            var blockNumberGreaterThan10Filter = new BlockFilter(
                b => Task.FromResult(b.Number.Value > 10));

            _blockFilters.Add(blockNumberGreaterThan10Filter);

            BlockProcessor blockProcessor = CreateBlockProcessor();

            const long blockNumber = 9;

            var stubBlock = new BlockWithTransactionHashes
            {
                Number = new HexBigInteger(blockNumber)
            };

            _mockBlockProxy
                .Setup(p => p.GetBlockWithTransactionsHashesByNumber(9))
                .ReturnsAsync(stubBlock);

            //execute
            await blockProcessor.ProcessBlockAsync(blockNumber);

            //assert
            _mockBlockRepository.Verify(b => b.UpsertBlockAsync(stubBlock), Times.Never);

            _mockTransactionProcessor
                .Verify(t => t.ProcessTransactionAsync(
                    It.IsAny<String>(), It.IsAny<BlockWithTransactionHashes>()), Times.Never);

        }

        [Fact]
        public async Task ProcessBlock_WhenBlockProxyReturnsNull_ThrowsBlockNotFoundException()
        {
            //setup
            const long blockNumber = 9;

            BlockProcessor blockProcessor = CreateBlockProcessor();

            _mockBlockProxy
                .Setup(p => p.GetBlockWithTransactionsHashesByNumber(9))
                .ReturnsAsync((BlockWithTransactionHashes)null);

            //execute
            await Assert.ThrowsAsync<BlockNotFoundException>(
                async () => await blockProcessor.ProcessBlockAsync(blockNumber));
        }

        private BlockProcessor CreateBlockProcessor()
        {
            return new BlockProcessor(
                _mockBlockProxy.Object, 
                _mockBlockRepository.Object, 
                _mockTransactionProcessor.Object, 
                _blockFilters);
        }
    }
}
