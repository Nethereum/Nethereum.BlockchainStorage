using Common.Logging;
using Nethereum.Contracts;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Filters;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class BlockRangeLogsProcessor : IBlockchainProcessor
    {
        private readonly BlockRangeLogsProcessorInstrumentation _logger;

        public uint MaxRetries { get; set; } = 3;

        public IWaitStrategy RetryWaitStrategy { get; set; } = new WaitStrategy();

        private readonly IEthGetLogs _eventLogProxy;
        private readonly IEnumerable<ILogProcessor> _logProcessors;
        private readonly List<NewFilterInput> _filters;

        public BlockRangeLogsProcessor(
            IWeb3 web3,
            IEnumerable<ILogProcessor> logProcessors,
            ILog log = null) : this(web3.Eth.Filters.GetLogs, logProcessors, filter: null, log: log)
        {
        }

        public BlockRangeLogsProcessor(
            IWeb3 web3,
            IEnumerable<ILogProcessor> logProcessors,
            NewFilterInput filter,
            ILog log = null) : this(web3.Eth.Filters.GetLogs, logProcessors, filter, log)
        {
        }

        public BlockRangeLogsProcessor(
            IWeb3 web3,
            IEnumerable<ILogProcessor> logProcessors,
            IEnumerable<NewFilterInput> filters,
            ILog log = null) : this(web3.Eth.Filters.GetLogs, logProcessors, filters, log)
        {
        }

        public BlockRangeLogsProcessor(
            IEthGetLogs eventLogProxy, 
            IEnumerable<ILogProcessor> logProcessors, 
            NewFilterInput filter,
            ILog log = null) :this(eventLogProxy, logProcessors, filter == null ? null : new NewFilterInput[]{filter}, log)
        {
        }

        public BlockRangeLogsProcessor(
            IEthGetLogs eventLogProxy, 
            IEnumerable<ILogProcessor> logProcessors, 
            IEnumerable<NewFilterInput> filters = null,
            ILog log = null)
        {
            _eventLogProxy = eventLogProxy ?? throw new ArgumentNullException(nameof(eventLogProxy));
            _logProcessors = logProcessors ?? throw new ArgumentNullException(nameof(logProcessors));

            _filters = filters?.ToList() ?? new List<NewFilterInput>();

            if (_filters.Count == 0)
            {
                _filters.Add(new NewFilterInput());
            }

            _logger = new BlockRangeLogsProcessorInstrumentation(log);
        }

        public Task ProcessAsync(BlockRange range)
        {
            return ProcessAsync(range, new CancellationToken());
        }

        public async Task ProcessAsync(BlockRange range, CancellationToken cancellationToken)
        {
            _logger.ProcessingRange(range);

            var distinctLogs = await RetrieveLogsAsync(range, cancellationToken)
                .ConfigureAwait(false);

            if (!distinctLogs.Any())
            {
                _logger.NoLogsToProcess(range);
                return;
            }

            if (cancellationToken.IsCancellationRequested) 
            {
                _logger.CancellationRequested();
                return;
            }

            var queues = Allocate(distinctLogs);

            await ProcessQueuesAsync(queues, cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task ProcessQueuesAsync(
            Dictionary<ILogProcessor, IEnumerable<FilterLog>> processorWorkQueue, 
            CancellationToken cancellationToken)
        {
            foreach (ILogProcessor processor in processorWorkQueue.Keys)
            {
                if (cancellationToken.IsCancellationRequested) 
                {
                    _logger.CancellationRequested();
                    return; 
                }

                var logsToProcess = processorWorkQueue[processor].ToArray();

                _logger.ProcessingLogs(processor, logsToProcess);

                await processor.ProcessLogsAsync(logsToProcess).ConfigureAwait(false);
            }
        }

        private Dictionary<ILogProcessor, IEnumerable<FilterLog>> Allocate(FilterLog[] logs)
        {
            _logger.AllocatingLogs(logs, _logProcessors);

            var queues = _logProcessors
                .ToDictionary(
                    (processor) => processor, //key
                    (processor) => logs.Where(processor.IsLogForEvent) // matching logs
                );

            _logger.LogsAllocated(queues);

            return queues;
        }

        private async Task<FilterLog[]> RetrieveLogsAsync(
            BlockRange range, CancellationToken cancellationToken)
        {
            var logs = new Dictionary<string, FilterLog>();

            foreach (var filter in _filters)
            {
                FilterLog[] logsMatchingFilter = await RetrieveLogsAsync(range, filter)
                    .ConfigureAwait(false);

                _logger.MergingLogs(logs, logsMatchingFilter);
                logs.Merge(logsMatchingFilter);

                if (cancellationToken.IsCancellationRequested) return logs.Values.Sort();
            }

            return logs.Values.Sort();
        }

        private async Task<FilterLog[]> RetrieveLogsAsync(
            BlockRange range, NewFilterInput filter, uint retryNumber = 0)
        {
            try
            {
                filter.SetBlockRange(range);

                _logger.RetrievingLogs(filter, range, retryNumber);

                return await _eventLogProxy.SendRequestAsync(filter).ConfigureAwait(false);
            }
            catch (RpcResponseException rpcResponseEx) when (rpcResponseEx.TooManyRecords())
            {
                _logger.TooManyRecords(range, rpcResponseEx);
                throw rpcResponseEx.TooManyRecordsException();
            }
            catch (Exception ex)
            {
                _logger.RetrievalError(range, ex);

                retryNumber++;
                if (retryNumber < MaxRetries)
                {
                    _logger.PausingBeforeRetry(range, retryNumber);
                    await RetryWaitStrategy.Apply(retryNumber).ConfigureAwait(false);

                    return await RetrieveLogsAsync(range, filter, retryNumber)
                        .ConfigureAwait(false);
                }

                _logger.MaxRetriesExceededSoThrowing(MaxRetries, ex);

                throw;
            }
        }
    }
}
