using Common.Logging;
using Nethereum.BlockchainProcessing.Common.Utils;
using System.Numerics;

namespace Nethereum.BlockchainProcessing.Processing
{
    public class BlockchainProcessorLogger : BaseLogger
    {
        public BlockchainProcessorLogger(ILog logger) : base(logger)
        {
        }

        public void GettingLastBlockProcessed()
        {
            if(IsInfoEnabled) Logger.Info("Begin GetStartingBlockNumber / _strategy.GetLastBlockProcessedAsync()");
        }

        public void LastBlockProcessed(BigInteger? lastBlock)
        {
            if (IsInfoEnabled) Logger.Info(lastBlock == null ? "No blocks previously processed" : $"Last Block: {lastBlock}");
        }

        public void BeginFillContractCacheAsync()
        {
            if(IsInfoEnabled) Logger.Info("Begin FillContractCacheAsync");
        }

        public void BeginningBlockEnumeration()
        {
            if (IsInfoEnabled) Logger.Info("Beginning Block Enumeration");
        }

        public void StartBlockExceedsEndBlock(BigInteger start, BigInteger end)
        {
            if(IsInfoEnabled) Logger.Info($"Start block '{start}' exceed ending block '{end}'.");
        }

    }
}