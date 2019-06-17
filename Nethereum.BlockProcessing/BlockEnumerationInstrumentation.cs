using System;
using System.Numerics;
using Common.Logging;
using Nethereum.BlockchainProcessing.Common.Utils;

namespace Nethereum.BlockchainProcessing.Processing
{
    public class BlockEnumerationInstrumentation : InstrumentationBase
    {
        public BlockEnumerationInstrumentation(ILog logger) : base(logger)
        {
        }

        public void LogProcessBlockAttempt(BigInteger currentBlock, uint retryNumber)
        {
            if(IsInfoEnabled) Logger.Info($"Block Process Attempt.  Block: {currentBlock}, Attempt: {retryNumber}.");
        }

        public void WaitingForBlockAvailability(BigInteger currentBlock, uint minimumBlockConfirmations, BigInteger? maxBlockOnChain, uint attempt)
        {
            if(IsInfoEnabled) Logger.Info($"Waiting for current block ({currentBlock}) to be more than {minimumBlockConfirmations} confirmations behind the max block on the chain ({maxBlockOnChain}). Attempt: {attempt}.");
        }

        public void WaitingForBlock(BigInteger block)
        {
            if(IsInfoEnabled) Logger.Info($"Waiting for block {block}...");
        }

        public void LogBlockSkipped(BigInteger block)
        {
            if(IsWarnEnabled) Logger.Warn($"Skipping block {block}");
        }

        public void PauseBeforeNextProcessingAttempt(BigInteger currentBlock, uint retryNumber)
        {
            if(IsInfoEnabled) Logger.Info($"Pausing before next process Attempt.  Block: {currentBlock}, Attempt: {retryNumber}.");
        }

        public void Error(BigInteger currentBlock, Exception exception)
        {
            if(IsErrorEnabled) Logger.Error($"Block: {currentBlock}.  {exception.Message}", exception);
        }

        public void BlockIncremented(BigInteger currentBlock)
        {
            if(IsInfoEnabled) Logger.Info($"Block Incremented, Current Block: {currentBlock}");
        }
    }

}