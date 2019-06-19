using Moq;
using Nethereum.BlockchainProcessing.Common.Tests;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processors;
using Nethereum.BlockchainProcessing.Processors.Transactions;
using System.Collections.Generic;

namespace Nethereum.BlockProcessing.Tests
{
    public class BlockProcessorTestBase
    {
        protected internal const ulong BlockNumber = 9;
        protected readonly Mock<IBlockHandler> MockBlockHandler;
        protected readonly Mock<ITransactionProcessor> MockTransactionProcessor;
        protected readonly List<IBlockFilter> BlockFilters;
        protected readonly BlockProcessor BlockProcessor;
        protected readonly Web3Mock Web3Mock;

        public BlockProcessorTestBase()
        {
            Web3Mock = new Web3Mock();
            MockBlockHandler = new Mock<IBlockHandler>();
            MockTransactionProcessor = new Mock<ITransactionProcessor>();
            BlockFilters = new List<IBlockFilter>();
            BlockProcessor = new BlockProcessor(
                Web3Mock.Web3,
                MockBlockHandler.Object,
                MockTransactionProcessor.Object,
                BlockFilters);
        }
    }
}
