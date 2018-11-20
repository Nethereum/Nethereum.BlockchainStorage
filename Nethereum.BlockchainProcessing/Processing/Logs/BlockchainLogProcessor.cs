using System;
using Nethereum.BlockchainProcessing.Web3Abstractions;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class BlockchainLogProcessor : IBlockchainProcessor
    {
        private readonly IEventLogProxy _eventLogProxy;
        private readonly IEnumerable<ILogProcessor> _logProcessors;
        private readonly NewFilterInput[] _filters;

        public BlockchainLogProcessor(
            IEventLogProxy eventLogProxy, 
            IEnumerable<ILogProcessor> logProcessors, 
            NewFilterInput filter):this(eventLogProxy, logProcessors, filter == null ? null : new NewFilterInput[]{filter})
        {
        }

        public BlockchainLogProcessor(
            IEventLogProxy eventLogProxy, 
            IEnumerable<ILogProcessor> logProcessors, 
            NewFilterInput[] filters = null)
        {
            _eventLogProxy = eventLogProxy ?? throw new ArgumentNullException(nameof(eventLogProxy));
            _logProcessors = logProcessors ?? throw new ArgumentNullException(nameof(logProcessors));
            _filters = filters == null || filters.Length == 0 ? filters =new []{ new NewFilterInput()} : filters;
        }

        public async Task ProcessAsync(ulong fromBlockNumber, ulong toBlockNumber)
        {
            await ProcessAsync(fromBlockNumber, toBlockNumber, new CancellationToken());
        }

        public async Task ProcessAsync(ulong fromBlockNumber, ulong toBlockNumber, CancellationToken cancellationToken)
        {
            var logs = new List<FilterLog>();

            foreach (var filter in _filters)
            {
                filter.FromBlock = new BlockParameter(fromBlockNumber);
                filter.ToBlock = new BlockParameter(toBlockNumber);

                logs.AddRange(await _eventLogProxy.GetLogs(filter));
            }

            if (logs.Count == 0) return;

            if (cancellationToken.IsCancellationRequested) return;

            var processorWorkQueue = _logProcessors
                .ToDictionary((processor) => processor, (processor) => logs.Where(processor.IsLogForEvent));

            foreach (var processor in processorWorkQueue.Keys)
            {
                if (cancellationToken.IsCancellationRequested) return;

                var logsToProcess = processorWorkQueue[processor].ToArray();
                await processor.ProcessLogsAsync(logsToProcess);
            }
        }
    }
}
