using System;
using Nethereum.BlockchainProcessing.Web3Abstractions;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nethereum.Configuration;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class BlockchainLogProcessor : IBlockchainProcessor
    {
        private readonly ILogger _log = ApplicationLogging.CreateLogger<BlockchainLogProcessor>();

        public uint MaxRetries { get; set; } = 3;

        public IWaitStrategy RetryWaitStrategy { get; set; } = new WaitStrategy();

        private readonly IEventLogProxy _eventLogProxy;
        private readonly IEnumerable<ILogProcessor> _logProcessors;
        private readonly List<NewFilterInput> _filters;

        public BlockchainLogProcessor(
            IEventLogProxy eventLogProxy, 
            IEnumerable<ILogProcessor> logProcessors, 
            NewFilterInput filter):this(eventLogProxy, logProcessors, filter == null ? null : new NewFilterInput[]{filter})
        {
        }

        public BlockchainLogProcessor(
            IEventLogProxy eventLogProxy, 
            IEnumerable<ILogProcessor> logProcessors, 
            IEnumerable<NewFilterInput> filters = null)
        {
            _eventLogProxy = eventLogProxy ?? throw new ArgumentNullException(nameof(eventLogProxy));
            _logProcessors = logProcessors ?? throw new ArgumentNullException(nameof(logProcessors));

            _filters = filters?.ToList() ?? new List<NewFilterInput>();

            if (_filters.Count == 0)
            {
                _filters.Add(new NewFilterInput());
            }
        }

        public Task ProcessAsync(BlockRange range)
        {
            return ProcessAsync(range, new CancellationToken());
        }

        public async Task ProcessAsync(BlockRange range, CancellationToken cancellationToken)
        {
            _log.LogInformation($"Beginning ProcessAsync. from: {range.From}, to: {range.To}.");
            _log.LogInformation("Retrieving logs");
            var distinctLogs = await RetrieveLogsAsync(range, cancellationToken)
                .ConfigureAwait(false);

            if (!distinctLogs.Any()) return;
            if (cancellationToken.IsCancellationRequested) return;

            _log.LogInformation("Allocating logs to processors");
            var queues = Allocate(distinctLogs);

            _log.LogInformation("Processing logs");
            await ProcessQueuesAsync(queues, cancellationToken)
                .ConfigureAwait(false);
        }

        private static async Task ProcessQueuesAsync(
            Dictionary<ILogProcessor, IEnumerable<FilterLog>> processorWorkQueue, 
            CancellationToken cancellationToken)
        {
            foreach (ILogProcessor processor in processorWorkQueue.Keys)
            {
                if (cancellationToken.IsCancellationRequested) return;

                var logsToProcess = processorWorkQueue[processor].ToArray();
                await processor.ProcessLogsAsync(logsToProcess).ConfigureAwait(false);
            }
        }

        private Dictionary<ILogProcessor, IEnumerable<FilterLog>> Allocate(FilterLog[] logs)
        {
            return _logProcessors
                .ToDictionary(
                    (processor) => processor, //key
                    (processor) => logs.Where(processor.IsLogForEvent) // matching logs
                );
        }

        private async Task<FilterLog[]> RetrieveLogsAsync(
            BlockRange range, CancellationToken cancellationToken)
        {
            var logs = new Dictionary<string, FilterLog>();

            foreach (var filter in _filters)
            {
                FilterLog[] logsMatchingFilter = await RetrieveLogsAsync(range, filter)
                    .ConfigureAwait(false);

                logs.Merge(logsMatchingFilter);

                if (cancellationToken.IsCancellationRequested) return logs.Values.ToArray();
            }

            return logs.Values.ToArray();
        }

        private async Task<FilterLog[]> RetrieveLogsAsync(
            BlockRange range, NewFilterInput filter, uint retryNumber = 0)
        {
            try
            {
                filter.SetBlockRange(range);

                _log.LogInformation($"RetrieveLogsAsync - getting logs. RetryNumber:{retryNumber}, from:{range.From}, to:{range.To}.");

                return await _eventLogProxy.GetLogs(filter).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _log.LogError("Get Logs Error", ex);

                retryNumber++;
                if (retryNumber < MaxRetries)
                {
                    _log.LogInformation("Pausing before retry get logs");
                    await RetryWaitStrategy.Apply(retryNumber).ConfigureAwait(false);

                    _log.LogInformation("Retrying get logs");
                    return await RetrieveLogsAsync(range, filter, retryNumber)
                        .ConfigureAwait(false);
                }

                _log.LogError("MaxRetries exceeded when getting logs, throwing exception.", ex);

                throw;
            }
        }
    }
}
