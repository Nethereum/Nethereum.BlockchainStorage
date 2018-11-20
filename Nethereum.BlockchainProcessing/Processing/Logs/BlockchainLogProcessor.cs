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

        public BlockchainLogProcessor(IEventLogProxy eventLogProxy, IEnumerable<ILogProcessor> logProcessors)
        {
            _eventLogProxy = eventLogProxy ?? throw new ArgumentNullException(nameof(eventLogProxy));
            _logProcessors = logProcessors ?? throw new ArgumentNullException(nameof(logProcessors));
        }

        public async Task ProcessAsync(ulong fromBlockNumber, ulong toBlockNumber)
        {
            await ProcessAsync(fromBlockNumber, toBlockNumber, new CancellationToken());
        }

        public async Task ProcessAsync(ulong fromBlockNumber, ulong toBlockNumber, CancellationToken cancellationToken)
        {
            var logs = await _eventLogProxy.GetLogs(new NewFilterInput
            {
                FromBlock = new BlockParameter(fromBlockNumber),
                ToBlock = new BlockParameter(toBlockNumber)
            });

            if (logs == null || logs.Length == 0) return;

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
