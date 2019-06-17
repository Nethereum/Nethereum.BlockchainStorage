using Common.Logging;
using Nethereum.BlockchainProcessing.Common.Utils;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class BlockRangeLogsProcessorInstrumentation : InstrumentationBase
    {
        public BlockRangeLogsProcessorInstrumentation(ILog logger) : base(logger)
        {
        }

        public void ProcessingRange(BlockRange range)
        {
            if(IsInfoEnabled) Logger.Info($"Beginning ProcessAsync. from: {range.From}, to: {range.To}.");
        }

        public void RetrievingLogs(NewFilterInput filter, BlockRange range, uint attemptNumber)
        {
            if (IsInfoEnabled) Logger.Info($"Retrieving logs. from: {range.From}, to: {range.To}. Attempt: {attemptNumber}. Filter: {JsonConvert.SerializeObject(filter)}");
        }

        public void PausingBeforeRetry(BlockRange range, uint attemptNumber)
        {
            if (IsInfoEnabled) Logger.Info($"Pausing before retrieval retry. from: {range.From}, to: {range.To}. Attempt: {attemptNumber}");
        }

        public void TooManyRecords(BlockRange range, Exception ex)
        {
            if (IsErrorEnabled) Logger.Error($"TooManyRecords exception whilst retrieving logs. from: {range.From}, to: {range.To}.", ex);
        }

        public void RetrievalError(BlockRange range, Exception ex)
        {
            if (IsErrorEnabled) Logger.Error($"Exception whilst retrieving logs. from: {range.From}, to: {range.To}.", ex);
        }

        public void MaxRetriesExceededSoThrowing(uint maxretries, Exception ex)
        {
            if(IsErrorEnabled) Logger.Error($"MaxRetries '{maxretries}' exceeded when getting logs, throwing exception.", ex);
        }

        public void AllocatingLogs(FilterLog[] logs, IEnumerable<ILogProcessor> logProcessors)
        {
            if(IsInfoEnabled) Logger.Info($"Allocating logs (Count:{logs.Length}) to processors (Count: {logProcessors.Count()})");
        }

        public void ProcessingLogs(ILogProcessor processor, FilterLog[] logs)
        {
            if(IsInfoEnabled) Logger.Info($"Processing Logs.  Processor: {processor.GetType().FullName}, Logs: {logs.Length}");
        }

        public void NoLogsToProcess(BlockRange range)
        {
            if (IsInfoEnabled) Logger.Info($"No logs to process. Blocks from: {range.From}, to: {range.To}.");
        }

        public void CancellationRequested()
        {
            if (IsInfoEnabled) Logger.Info($"Cancellation Requested");
        }

        public void LogsAllocated(Dictionary<ILogProcessor, IEnumerable<FilterLog>> queues)
        {
            if (IsInfoEnabled) 
            { 
                Logger.Info($"Logs Allocated to processors. Processors with Logs To Process: {queues.Keys.Count}"); 
                foreach(var processor in queues.Keys)
                {
                    Logger.Info($"Logs Allocated to Processor: {processor.GetType().FullName}, Logs: {queues[processor]?.Count()}");
                }
            }
        }

        public void MergingLogs(Dictionary<string, FilterLog> logs, FilterLog[] logsMatchingFilter)
        {
            if(IsInfoEnabled) Logger.Info($"Merging {logsMatchingFilter.Length} logs into master list(current length: {logs.Count})");
        }
    }
}
