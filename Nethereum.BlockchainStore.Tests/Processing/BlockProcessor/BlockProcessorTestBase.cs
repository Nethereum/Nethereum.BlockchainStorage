using Moq;
using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.Web3Abstractions;
using System.Collections.Generic;

namespace Nethereum.BlockchainStore.Tests.Processing.BlockProcessorTests
{
    public class BlockProcessorTestBase
    {
        protected internal const long BlockNumber = 9;
        protected readonly Mock<IGetBlockWithTransactionHashesByNumber> MockBlockProxy;
        protected readonly Mock<IBlockRepository> MockBlockRepository;
        protected readonly Mock<ITransactionProcessor> MockTransactionProcessor;
        protected readonly List<IBlockFilter> BlockFilters;
        protected readonly BlockProcessor BlockProcessor;

        public BlockProcessorTestBase()
        {
            MockBlockProxy = new Mock<IGetBlockWithTransactionHashesByNumber>();
            MockBlockRepository = new Mock<IBlockRepository>();
            MockTransactionProcessor = new Mock<ITransactionProcessor>();
            BlockFilters = new List<IBlockFilter>();
            BlockProcessor = new BlockProcessor(
                MockBlockProxy.Object,
                MockBlockRepository.Object,
                MockTransactionProcessor.Object,
                BlockFilters);
        }
    }
}
