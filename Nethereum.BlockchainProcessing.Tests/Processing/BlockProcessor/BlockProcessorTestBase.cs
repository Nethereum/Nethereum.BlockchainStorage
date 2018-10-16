using Moq;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processors;
using Nethereum.BlockchainProcessing.Processors.Transactions;
using Nethereum.BlockchainProcessing.Web3Abstractions;
using System.Collections.Generic;

namespace Nethereum.BlockchainStore.Tests.Processing.BlockProcessorTests
{
    public class BlockProcessorTestBase
    {
        protected internal const long BlockNumber = 9;
        protected readonly Mock<IBlockProxy> MockBlockProxy;
        protected readonly Mock<IBlockHandler> MockBlockHandler;
        protected readonly Mock<ITransactionProcessor> MockTransactionProcessor;
        protected readonly List<IBlockFilter> BlockFilters;
        protected readonly BlockProcessor BlockProcessor;

        public BlockProcessorTestBase()
        {
            MockBlockProxy = new Mock<IBlockProxy>();
            MockBlockHandler = new Mock<IBlockHandler>();
            MockTransactionProcessor = new Mock<ITransactionProcessor>();
            BlockFilters = new List<IBlockFilter>();
            BlockProcessor = new BlockProcessor(
                MockBlockProxy.Object,
                MockBlockHandler.Object,
                MockTransactionProcessor.Object,
                BlockFilters);
        }
    }
}
