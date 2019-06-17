using System;
using Nethereum.BlockchainProcessing;
using Nethereum.Contracts;
using Common.Logging;
using Nethereum.BlockchainProcessing.Common.Utils;

namespace Nethereum.LogProcessing
{
    public class LogsProcessorLogger: BaseLogger
    {
        public LogsProcessorLogger(ILog logger):base(logger)
        {
        }

        public void RetrievingBlockNumberRange()
        {
            if (IsTraceEnabled) Logger.Trace("Getting block number range to process");
        }

        public void NoBlocksToProcess()
        {
            if(IsInfoEnabled) Logger.Info("No block range to process - the most recent block may already have been processed");
        }

        public void ProcessingBlockRange(BlockRange range)
        {
            if(IsInfoEnabled) Logger.Info($"Processing Block Range.from: { range.From} to { range.To}");
        }

        public void UpdatingBlockProgress(BlockRange range)
        {
            if(IsInfoEnabled) Logger.Info($"Updating current process progress to: {range.To}");
        }

        public void TooManyRecords(TooManyRecordsException ex)
        {
            if(IsWarnEnabled) Logger.Warn($"Too many results error. : {ex.Message}");
        }

        public void ChangingBlocksPerBatch(uint oldVal, uint newVal)
        {
            if (IsWarnEnabled) Logger.Warn($"Resetting _maxNumberOfBlocksPerBatch. Old Value:{oldVal}, New Value: {newVal}");
        }

        public void FatalError(Exception ex)
        {
            if(IsErrorEnabled) Logger.Error("Logs Processor Fatal Error.", ex);
        }
    }
}
