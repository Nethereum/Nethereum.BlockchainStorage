using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.Configuration;
using Nethereum.Contracts;
using Nethereum.Contracts.Services;
using Nethereum.Hex.HexTypes;
using Nethereum.LogProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.PerformanceTests
{


    class Program
    {
        private static readonly ILogger _log = ApplicationLogging.CreateLogger<Program>();

        static async Task Main(string[] args)
        {
            try
            {
                Config.LogOutputToConsole();
                _log.LogInformation("Starting");

                var test = new WritingTransfersToTheConsole(numberOfBlocksToProcess: 171_000, maxDuration: TimeSpan.FromHours(2), maxBlocksPerBatch: 1000);
                //var test = new WritingTransfersToTheAzureStorage(
                //    Config.AzureConnectionString, 
                //    "perfTest", 
                //    numberOfBlocksToProcess: 171_000, 
                //    maxDuration: TimeSpan.FromHours(5), 
                //    maxBlocksPerBatch: 100);
                //WritingTransfersToTheAzureStorage
                await test.RunTestAsync();
            }
            catch(Exception ex)
            {
                _log.LogError(ex.ToString());
            }

            Console.ReadLine();
        }
    }

    public class Config
    {
        static IConfigurationRoot _config;

        public static void LogOutputToConsole()
        {
            Configuration.AddConsoleLogging();
        }

        public static IConfigurationRoot Configuration
        {
            get
            {
                if(_config == null)
                {
                    ConfigurationUtils.SetEnvironment("development");

                    //use the command line to set your azure search api key
                    //e.g. dotnet user-secrets set "AzureStorageConnectionString" "<put key here>"
                    _config = ConfigurationUtils
                        .Build(Array.Empty<string>(), userSecretsId: "Nethereum.BlockchainProcessing.PerformanceTests");
                }
                return _config;
            }
        }

        public static string AzureConnectionString => Configuration["AzureStorageConnectionString"];
    }

    public abstract class PerfTest
    {
        protected static readonly ILogger _log = ApplicationLogging.CreateLogger<PerfTest>();

        public virtual Task ConfigureAsync() => Task.CompletedTask;
        protected abstract Task RunAsync();

        protected Stopwatch stopWatch;

        public virtual async Task RunTestAsync()
        {
            await ConfigureAsync();
            stopWatch = Stopwatch.StartNew();
            await RunAsync();
            var elapsed = stopWatch.Elapsed;
            stopWatch.Stop();

            _log.LogInformation("** Finished");
            _log.LogInformation($"** Elapsed: Hours: {elapsed.Hours}, Minutes: {elapsed.Minutes}, Seconds: {elapsed.Seconds}, Ms: {elapsed.Milliseconds}");
        }
    }

    public class WritingTransfersToTheAzureStorage : PerfTest
    {
        public int EventsHandled = 0;
        public HashSet<HexBigInteger> BlocksContainingTransfers = new HashSet<HexBigInteger>();
        private ILogsProcessor _processor;
        private CancellationTokenSource cancellationTokenSource;
        private ulong MaxBlockNumber;
        private ulong BlocksProcessed;
        private ulong LastBlock;
        private ILogsProcessorBuilder _builder;
        private IEthApiContractService ethApiContractService;

        public WritingTransfersToTheAzureStorage(string azureConnectionString, string tablePrefix, uint numberOfBlocksToProcess, TimeSpan maxDuration, uint maxBlocksPerBatch)
        {
            cancellationTokenSource = new CancellationTokenSource(maxDuration);
            _builder = new LogsProcessorBuilder("https://rinkeby.infura.io/v3/7238211010344719ad14a89db874158c")
                .Filter<TransferEventDto>()
                .StoreInAzureTable(azureConnectionString, tablePrefix, (Predicate<EventLog<TransferEventDto>>)((tfr) => TransferCallback(tfr)))
                .OnBatchProcessed((args) => Output(args.LastRangeProcessed))
                .SetBlocksPerBatch(maxBlocksPerBatch);

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

    public class WritingTransfersToTheConsole: PerfTest
    {
        public int EventsHandled = 0;
        public HashSet<HexBigInteger> BlocksContainingTransfers = new HashSet<HexBigInteger>();
        private ILogsProcessor _processor;
        private CancellationTokenSource cancellationTokenSource;
        private ulong MaxBlockNumber; 
        private ulong BlocksProcessed;
        private ulong LastBlock;
        private ILogsProcessorBuilder _builder;
        private IEthApiContractService ethApiContractService;

        public WritingTransfersToTheConsole(uint numberOfBlocksToProcess, TimeSpan maxDuration, uint maxBlocksPerBatch)
        {
            cancellationTokenSource = new CancellationTokenSource(maxDuration);
            _builder = new LogsProcessorBuilder("https://rinkeby.infura.io/v3/7238211010344719ad14a89db874158c")
                .Filter<TransferEventDto>()
                .Add<TransferEventDto>((events) => Output(events))
                .OnBatchProcessed((args) =>
                {
                    HandleBatchProcessed(args.LastRangeProcessed);
                })
                .SetBlocksPerBatch(maxBlocksPerBatch);

            ethApiContractService = _builder.Eth;
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
            foreach(var e in events)
            {
                _log.LogInformation($"\tBlock: {e.Log.BlockNumber.Value}, Hash: {e.Log.TransactionHash}, Index: {e.Log.LogIndex.Value}, From: {e.Event.From}, To: {e.Event.To}, Value: {e.Event.Value}");
                EventsHandled++;
                BlocksContainingTransfers.Add(e.Log.BlockNumber);
            }
            _log.LogInformation($"Blocks: {BlocksProcessed}, Last Block: {LastBlock}, Blocks With Transfers: {BlocksContainingTransfers.Count}, Transfers: {EventsHandled}");
        }

    }

    [Event("Transfer")]
    public class TransferEventDto : IEventDTO
    {
        [Parameter("address", "_from", 1, true)]
        public string From { get; set; }

        [Parameter("address", "_to", 2, true)]
        public string To { get; set; }

        [Parameter("uint256", "_value", 3, false)]
        public BigInteger Value { get; set; }
    }
}
