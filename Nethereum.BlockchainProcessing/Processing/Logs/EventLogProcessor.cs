using Microsoft.Extensions.Logging;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.Configuration;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{

    public class EventLogProcessor : IEventLogProcessor
    {
        ILogger _logger = ApplicationLogging.CreateLogger<EventLogProcessor>();

        public event EventHandler OnDisposing;

        public EventLogProcessor(string blockchainUrl) :
            this(new Web3.Web3(blockchainUrl))
        { }

        public EventLogProcessor(string blockchainUrl, string contractAddress) :
            this(new Web3.Web3(blockchainUrl), contractAddress)
        { }

        public EventLogProcessor(string blockchainUrl, string[] contractAddresses) :
            this(new Web3.Web3(blockchainUrl), contractAddresses)
        { }

        public EventLogProcessor(Web3.IWeb3 web3) :
            this(new BlockchainProxyService(web3), contractAddresses: null)
        { }
        public EventLogProcessor(Web3.IWeb3 web3, string contractAddress) :
            this(new BlockchainProxyService(web3), new[] { contractAddress })
        { }

        public EventLogProcessor(Web3.IWeb3 web3, string[] contractAddresses) :
            this(new BlockchainProxyService(web3), contractAddresses)
        { }

        public EventLogProcessor(IBlockchainProxyService blockchainProxyService, string[] contractAddresses = null)
        {
            BlockchainProxyService = blockchainProxyService;
            ContractAddresses = contractAddresses;

            if (ContractAddresses != null)
            {
                AddContractAddressFilter(ContractAddresses);
            }
        }

        public string[] ContractAddresses { get; }

        public Action<Exception> FatalErrorCallback { get; set; }

        public Action<uint, BlockRange> RangesProcessedCallback { get; set; }

        /// <summary>
        /// The earliest block to start at - important when there has been no prior processing
        /// </summary>
        public uint? MinimumBlockNumber { get; set; }

        /// <summary>
        /// eEnsure that new blocks aren't processed until the miniumum number of confirmations have been exceeded
        /// </summary>
        public uint? MinimumBlockConfirmations { get; set; }

        /// <summary>
        /// Each iteration processes a block range - this defines the number of blocks in that range
        /// </summary>
        public uint? MaximumBlocksPerBatch { get; set; }

        public IBlockProgressRepository BlockProgressRepository { get; set; }

        public IBlockchainProxyService BlockchainProxyService { get; set; }

        public List<ILogProcessor> Processors { get; set; } = new List<ILogProcessor>();

        public List<NewFilterInput> Filters { get; set; } = new List<NewFilterInput>();

        public IEventLogProcessor Configure(Action<IEventLogProcessor> configAction)
        {
            configAction(this);
            return this;
        }

        public IEventLogProcessor OnFatalError(Action<Exception> callBack)
        {
            FatalErrorCallback = callBack;
            return this;
        }

        public IEventLogProcessor Subscribe<TEventDto>(Action<IEnumerable<EventLog<TEventDto>>> callBack) where TEventDto : class, new()
        {
            var asyncCallback = new Func<IEnumerable<EventLog<TEventDto>>, Task>(async (events) => await Task.Run(() => callBack(events)).ConfigureAwait(false));
            return Subscribe(asyncCallback);
        }

        public IEventLogProcessor Subscribe<TEventDto>(Func<IEnumerable<EventLog<TEventDto>>, Task> callBack) where TEventDto : class, new()
        {
            Processors.Add(new LogProcessor<TEventDto>(callBack));
            return this;
        }

        public IEventLogProcessor SubscribeAndQueue<TEventDto>(IQueue queue, Predicate<EventLog<TEventDto>> predicate = null, Func<EventLog<TEventDto>, object> mapper = null) where TEventDto : class, new()
        {
            Processors.Add(new EventLogQueueProcessor<TEventDto>(queue, predicate, mapper));
            return this;
        }

        public IEventLogProcessor Subscribe(EventSubscription eventSubscription)
        {
            Processors.Add(eventSubscription);
            return this;
        }

        public IEventLogProcessor Subscribe<TEventDto>(EventSubscription<TEventDto> eventSubscription) where TEventDto : class, new()
        {
            Processors.Add(eventSubscription);
            return this;
        }

        public IEventLogProcessor Subscribe(ILogProcessor processor)
        {
            Processors.Add(processor);
            return this;
        }

        public IEventLogProcessor CatchAll(Action<IEnumerable<FilterLog>> callBack)
        {
            var asyncCallback = new Func<IEnumerable<FilterLog>, Task>(async (events) => await Task.Run(() => callBack(events)).ConfigureAwait(false));
            return CatchAll(asyncCallback);
        }

        public IEventLogProcessor CatchAll(Func<IEnumerable<FilterLog>, Task> callBack)
        {
            Processors.Add(new CatchAllLogProcessor(callBack));
            return this;
        }

        public IEventLogProcessor CatchAllAndQueue(IQueue queue, Predicate<FilterLog> predicate = null, Func<FilterLog, object> mapper = null)
        {
            Processors.Add(new EventLogQueueProcessor(queue, predicate, mapper));
            return this;
        }

        public IEventLogProcessor OnBatchProcessed(Action<uint, BlockRange> rangesProcessedCallback)
        {
            RangesProcessedCallback = rangesProcessedCallback;
            return this;
        }

        public IEventLogProcessor UseBlockProgressRepository(IBlockProgressRepository repo)
        {
            BlockProgressRepository = repo;
            return this;
        }

        public IEventLogProcessor UseJsonFileForBlockProgress(string jsonFilePath, bool deleteExistingFile = false)
        {
            BlockProgressRepository = new JsonBlockProgressRepository(jsonFilePath, deleteExistingFile: deleteExistingFile);
            return this;
        }

        /// <summary>
        /// Adds a filter based on the event signature
        /// For each filter in the processor a separate query is performed to request matching logs
        /// The logs for each filter are amalgamated in the retrieval stage
        /// Without filters - any log in the block range is retrieved and evaluated
        /// If filters are present - a log has to match atleast one filter (not all)
        /// </summary>
        /// <typeparam name="TEventDto"></typeparam>
        /// <returns></returns>
        public IEventLogProcessor Filter<TEventDto>() where TEventDto : class, IEventDTO, new()
        {
            Filters.Add(new NewFilterInputBuilder<TEventDto>().Build(ContractAddresses));
            return this;
        }

        public IEventLogProcessor Filter(NewFilterInput filter)
        {
            Filters.Add(filter);
            return this;
        }

        private void AddContractAddressFilter(string[] contractAddresses)
        {
            Filters.Add(new NewFilterInput { Address = contractAddresses });
        }

        private async Task<BlockchainBatchProcessorService> BuildService()
        {
            if (Processors == null || Processors.Count == 0) throw new ArgumentNullException(nameof(Processors));
            if (BlockchainProxyService == null) throw new ArgumentNullException(nameof(BlockchainProxyService));

            var startingBlock = MinimumBlockNumber ?? await BlockchainProxyService.GetMaxBlockNumberAsync().ConfigureAwait(false);

            BlockProgressRepository = BlockProgressRepository ?? new InMemoryBlockchainProgressRepository(startingBlock);
            var progressService = new BlockProgressService(BlockchainProxyService, startingBlock, BlockProgressRepository, MinimumBlockConfirmations ?? 0);
            var processor = new BlockchainLogProcessor(BlockchainProxyService, Processors, Filters?.ToArray());
            var batchProcessorService = new BlockchainBatchProcessorService(processor, progressService, MaximumBlocksPerBatch);

            return batchProcessorService;
        }

        /// <summary>
        /// Runs one process iteration as a blocking operation
        /// Would normally be called by a timed basis (i.e. web job)
        /// It will process a single batch limited by MaximumBlocksPerBatch from where it last processed 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<BlockRange?> RunForLatestBlocksAsync(
            CancellationToken cancellationToken)
        {
            var batchProcessorService = await BuildService().ConfigureAwait(false);
            return await batchProcessorService.ProcessLatestBlocksAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs on the current thread until cancellation 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ulong> RunAsync(
            CancellationToken cancellationToken)
        {
            var batchProcessorService = await BuildService().ConfigureAwait(false);
            return await batchProcessorService.ProcessContinuallyAsync(cancellationToken, RangesProcessedCallback).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs on a background thread until cancellation
        /// Returns a Task wrapper for the background Task
        /// Usage: await processor.RunInBackgroundAsync(ctx)
        /// awaiting ensures that any initial setup errors are caught on the calling thread
        /// once processing begins - it is on a non blocking background thread
        /// </summary>
        public async Task<Task> RunInBackgroundAsync(
            CancellationToken cancellationToken)
        {
            try
            {
                var batchProcessorService = await BuildService().ConfigureAwait(false);

                return Task.Factory
                    .StartNew
                    (
                        async () => await batchProcessorService.ProcessContinuallyAsync(cancellationToken, RangesProcessedCallback),
                        cancellationToken,
                        TaskCreationOptions.LongRunning,
                        TaskScheduler.Default
                    )
                    .ContinueWith<ulong>
                    (
                        (t) =>
                        {

                            if (t.IsFaulted)
                            {
                                _logger.LogError(t.Exception, t.Exception.GetBaseException().Message);
                                FatalErrorCallback?.Invoke(t.Exception.GetBaseException());
                            }

                            return t.Result.Result;

                        },
                        TaskContinuationOptions.OnlyOnFaulted
                    );

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(EventLogProcessor)}.RunInBackgroundAsync threw an initialisation error");
                throw;
            }
        }

        public void Dispose()
        {
            OnDisposing?.Invoke(this, new EventArgs());
        }
    }
}
