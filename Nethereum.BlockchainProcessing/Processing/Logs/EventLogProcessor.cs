using Microsoft.Extensions.Logging;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.Configuration;
using Nethereum.Contracts;
using Nethereum.Contracts.Services;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{

    public class EventLogProcessor : IEventLogProcessor
    {
        private NewFilterInput _contractAddressFilter;
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

        public EventLogProcessor(IEthApiContractService eth, string[] contractAddresses) :
            this(new BlockchainProxyService(eth), contractAddresses)
        { }

        public EventLogProcessor(IBlockchainProxyService blockchainProxyService, string[] contractAddresses = null)
        {
            BlockchainProxyService = blockchainProxyService ?? throw new ArgumentNullException(nameof(blockchainProxyService));
            ContractAddresses = contractAddresses;

            if (ContractAddresses != null)
            {
                SetContractAddressFilter(ContractAddresses);
            }
        }

        public string[] ContractAddresses { get; }

        public Action<Exception> FatalErrorCallback { get; set; }

        public Action<uint, BlockRange> RangesProcessedCallback { get; set; }

        /// <summary>
        /// The earliest block to start at - important when there has been no prior processing
        /// </summary>
        public ulong? MinimumBlockNumber { get; set; }

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

        public List<ILogProcessor> Processors { get; private set; } = new List<ILogProcessor>();

        public List<NewFilterInput> Filters { get; private set; } = new List<NewFilterInput>();

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
            //if we're already filtering for contract addresses but not by event
            //clear the existing filter so we can add a new one which is event specific

            var filter = new NewFilterInputBuilder<TEventDto>().Build(ContractAddresses);

            if (_contractAddressFilter != null )
            {
                Filters.Remove(_contractAddressFilter);
                _contractAddressFilter = filter;
            }

            Filters.Add(filter);
            return this;
        }

        public IEventLogProcessor Filter(NewFilterInput filter)
        {
            Filters.Add(filter);
            return this;
        }

        private void SetContractAddressFilter(string[] contractAddresses)
        {
            _contractAddressFilter = new NewFilterInput { Address = contractAddresses };
            Filters.Add(_contractAddressFilter);
        }

        private async Task<BlockchainBatchProcessorService> BuildService()
        {
            if (Processors == null || Processors.Count == 0) throw new ArgumentNullException(nameof(Processors));
            if (BlockchainProxyService == null) throw new ArgumentNullException(nameof(BlockchainProxyService));

            var startingBlock = MinimumBlockNumber ?? await BlockchainProxyService.GetMaxBlockNumberAsync().ConfigureAwait(false);

            var lastBlockProcessed = startingBlock == 0 ? (ulong?)null : startingBlock - 1;

            BlockProgressRepository = BlockProgressRepository ?? new InMemoryBlockchainProgressRepository(lastBlockProcessed);
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
        public async Task<BlockRange?> RunOnceAsync(
            CancellationToken cancellationToken)
        {
            try 
            { 
                var batchProcessorService = await BuildService().ConfigureAwait(false);
                return await batchProcessorService.ProcessOnceAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                FatalErrorCallback?.Invoke(ex);
                throw;
            }
        }

        /// <summary>
        /// Runs on the current thread until cancellation 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ulong> RunAsync(
            CancellationToken cancellationToken)
        {
            try 
            { 
                var batchProcessorService = await BuildService().ConfigureAwait(false);
                return await batchProcessorService.ProcessContinuallyAsync(cancellationToken, RangesProcessedCallback).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                FatalErrorCallback?.Invoke(ex);
                throw;
            }
        }

        /// <summary>
        /// Runs on a background thread until cancellation or fatal error
        /// Returns a Task wrapper for the background Task
        /// Usage: var backgroundProcessingTask = await processor.RunInBackgroundAsync(ctx)
        /// awaiting ensures that any initial setup errors are caught on the calling thread
        /// once processing begins - it is on the non blocking background thread
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
                    .Unwrap()
                    .ContinueWith
                    (
                        (t) =>
                        {
                            if (t.IsFaulted)
                            {
                                var baseEx = t.Exception.GetBaseException();
                                _logger.LogError(baseEx, baseEx.Message);
                                FatalErrorCallback?.Invoke(baseEx);
                            }
                        },
                        TaskContinuationOptions.OnlyOnFaulted
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(EventLogProcessor)}.RunInBackgroundAsync threw an initialisation error");
                FatalErrorCallback?.Invoke(ex);
                throw;
            }
        }

        public void Dispose()
        {
            OnDisposing?.Invoke(this, new EventArgs());
        }
    }
}
