using Microsoft.Extensions.Logging;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.Contracts;
using Nethereum.Contracts.Services;
using Nethereum.Hex.HexTypes;
using Nethereum.LogProcessing;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.PerformanceTests
{
    public class WritingTransfersToTheAzureStorage : PerfTest
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

        public WritingTransfersToTheAzureStorage(string azureConnectionString, string tablePrefix, uint numberOfBlocksToProcess, TimeSpan maxDuration, uint maxBlocksPerBatch)
        {
            cancellationTokenSource = new CancellationTokenSource(maxDuration);
            _builder = new LogsProcessorBuilder("https://rinkeby.infura.io/v3/7238211010344719ad14a89db874158c")
                .Filter<TransferEventDto>()
                .StoreInAzureTable(azureConnectionString, tablePrefix, (Predicate<EventLog<TransferEventDto>>)((tfr) => TransferCallback(tfr)))
                .OnBatchProcessed((args) => Output(args.LastRangeProcessed))
                .SetBlocksPerBatch(maxBlocksPerBatch)
                .SetLog(_log.ToILog());

            ethApiContractService = _builder.Eth;

            NumberOfBlocksToProcess = numberOfBlocksToProcess;
        }

        private bool TransferCallback(EventLog<TransferEventDto> tfr)
        {
            EventsHandled++; 
            BlocksContainingTransfers.Add(tfr.Log.BlockNumber); return true;
        }

        private void Output(BlockRange lastRange)
        {
            BlocksProcessed += lastRange.BlockCount; LastBlock = lastRange.To; 
            if (lastRange.To >= MaxBlockNumber) { cancellationTokenSource.Cancel(); }

            var elapsed = stopWatch.Elapsed;
            _log.LogInformation($"** ELAPSED: Hours: {elapsed.Hours}, Minutes: {elapsed.Minutes}, Seconds: {elapsed.Seconds}");
            _log.LogInformation($"** PROGRESS: Blocks: {BlocksProcessed}, Last Block: {LastBlock}, Blocks With Transfers: {BlocksContainingTransfers.Count}, Transfers: {EventsHandled}");
        }

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

    }
}
