using Common.Logging;
using Microsoft.Extensions.Logging;
using Nethereum.Contracts;
using Nethereum.Contracts.Services;
using Nethereum.Hex.HexTypes;
using Nethereum.LogProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.PerformanceTests
{
    public class WritingTransfersToTheConsole: PerfTest
    {
        public int EventsHandled = 0;
        public HashSet<HexBigInteger> BlocksContainingTransfers = new HashSet<HexBigInteger>();
        private ILogsProcessor _processor;
        private CancellationTokenSource cancellationTokenSource;
        private BigInteger MaxBlockNumber; 
        private BigInteger BlocksProcessed;
        private BigInteger LastBlock;
        private ILogsProcessorBuilder _builder;
        private IEthApiContractService ethApiContractService;

        public WritingTransfersToTheConsole(string url, ILog log, uint numberOfBlocksToProcess, TimeSpan maxDuration, uint maxBlocksPerBatch)
        {
            cancellationTokenSource = new CancellationTokenSource(maxDuration);
            _builder = new LogsProcessorBuilder<TransferEventDto>(url)
                .OnEvents((events) => Output(events))
                .OnBatchProcessed((args) =>
                {
                    HandleBatchProcessed(args.LastRangeProcessed);
                })
                .SetBlocksPerBatch(maxBlocksPerBatch)
                .SetLog(log);

            ethApiContractService = _builder.Eth;
            Log = log;
            NumberOfBlocksToProcess = numberOfBlocksToProcess;
        }

        private void HandleBatchProcessed(BlockRange lastRange)
        {
            BlocksProcessed += lastRange.BlockCount;
            LastBlock = lastRange.To;
            if (lastRange.To >= MaxBlockNumber)
            {
                cancellationTokenSource.Cancel();
            }
        }

        public ILog Log { get; }
        public uint NumberOfBlocksToProcess { get; }

        public override async Task ConfigureAsync()
        {
            MaxBlockNumber = (await ethApiContractService.Blocks.GetBlockNumber.SendRequestAsync().ConfigureAwait(false)).ToUlong();
            _builder.SetMinimumBlockNumber(MaxBlockNumber - NumberOfBlocksToProcess);
            _processor = _builder.Build();
        }

        protected override async Task RunAsync()
        {
            await _processor.ProcessContinuallyAsync(cancellationTokenSource.Token);
        }

        protected virtual void Output(IEnumerable<EventLog<TransferEventDto>> events)
        {
            events = events ?? Array.Empty<EventLog<TransferEventDto>>();

            _log.LogInformation(DateTime.Now.ToLongTimeString());

            if (events.Any())
            {
                _log.LogInformation($"EVENTS FOUND: {events.Count()}");
            }

            foreach(var e in events)
            {
                _log.LogInformation($"\tBlock: {e.Log.BlockNumber.Value}, Hash: {e.Log.TransactionHash}, Index: {e.Log.LogIndex.Value}, From: {e.Event.From}, To: {e.Event.To}, Value: {e.Event.Value}");
                EventsHandled++;
                BlocksContainingTransfers.Add(e.Log.BlockNumber);
            }
            _log.LogInformation($"Blocks: {BlocksProcessed}, Last Block: {LastBlock}, Blocks With Transfers: {BlocksContainingTransfers.Count}, Transfers: {EventsHandled}");
        }

    }
}
