using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.Contracts;
using Nethereum.Contracts.Services;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class LogsProcessorBuilder<TEventDto> : LogsProcessorBuilder where TEventDto : class, IEventDTO, new()
    {

        public LogsProcessorBuilder(IEthApiContractService ethApiContractService) 
            : base(new BlockchainProxyService(ethApiContractService), new NewFilterInputBuilder<TEventDto>().Build())
        {
        }

        public LogsProcessorBuilder(IEthApiContractService ethApiContractService, Action<NewFilterInputBuilder<TEventDto>> configureFilterBuilder)
            : base(new BlockchainProxyService(ethApiContractService))
        {
            var filterBuilder = new NewFilterInputBuilder<TEventDto>();
            configureFilterBuilder(filterBuilder);
            Filters.Add(filterBuilder.Build());
        }

        public LogsProcessorBuilder(IEthApiContractService ethApiContractService, string contractAddress) 
            : base(new BlockchainProxyService(ethApiContractService), new NewFilterInputBuilder<TEventDto>().Build(contractAddress))
        {
        }

        public LogsProcessorBuilder(IEthApiContractService ethApiContractServicem, string[] contractAddresses) 
            : base(new BlockchainProxyService(ethApiContractServicem), new NewFilterInputBuilder<TEventDto>().Build(contractAddresses))
        {
        }

        public LogsProcessorBuilder<TEventDto> OnEvents(Action<IEnumerable<EventLog<TEventDto>>> callBack)
        {
            var asyncCallback = new Func<IEnumerable<EventLog<TEventDto>>, Task>(async (events) => await Task.Run(() => callBack(events)).ConfigureAwait(false));
            return OnEvents(asyncCallback);
        }

        public LogsProcessorBuilder<TEventDto> OnEvents(Func<IEnumerable<EventLog<TEventDto>>, Task> callBack)
        {
            Processors.Add(new LogProcessor<TEventDto>(callBack));
            return this;
        }
    }

    public class LogsProcessorBuilder
    {
        private NewFilterInput _contractAddressFilter;

        public Stack<IDisposable> _disposalStack = new Stack<IDisposable>();

        public LogsProcessorBuilder(string blockchainUrl) :
            this(new Web3.Web3(blockchainUrl))
        { }

        public LogsProcessorBuilder(string blockchainUrl, string contractAddress) :
            this(new Web3.Web3(blockchainUrl), contractAddress)
        { }

        public LogsProcessorBuilder(string blockchainUrl, string[] contractAddresses) :
            this(new Web3.Web3(blockchainUrl), contractAddresses)
        { }

        public LogsProcessorBuilder(Web3.IWeb3 web3) :
            this(new BlockchainProxyService(web3), contractAddresses: null)
        { }
        public LogsProcessorBuilder(Web3.IWeb3 web3, string contractAddress) :
            this(new BlockchainProxyService(web3), new[] { contractAddress })
        { }

        public LogsProcessorBuilder(Web3.IWeb3 web3, string[] contractAddresses) :
            this(new BlockchainProxyService(web3), contractAddresses)
        { }

        public LogsProcessorBuilder(IEthApiContractService eth, string contractAddress) :
            this(new BlockchainProxyService(eth), string.IsNullOrEmpty(contractAddress) ? null : new[]{ contractAddress})
        { }

        public LogsProcessorBuilder(IEthApiContractService eth, string[] contractAddresses) :
            this(new BlockchainProxyService(eth), contractAddresses)
        { }

        public LogsProcessorBuilder(IEthApiContractService eth, params NewFilterInput[] filters) :
            this(new BlockchainProxyService(eth), filters)
        { }

        public LogsProcessorBuilder(IEthApiContractService eth) :
            this(new BlockchainProxyService(eth), contractAddresses: null)
        { }


        public LogsProcessorBuilder(IBlockchainProxyService blockchainProxyService, string[] contractAddresses = null)
        {
            BlockchainProxyService = blockchainProxyService ?? throw new ArgumentNullException(nameof(blockchainProxyService));
            ContractAddresses = contractAddresses;

            if (ContractAddresses != null)
            {
                SetContractAddressFilter(ContractAddresses);
            }
        }

        public LogsProcessorBuilder(IBlockchainProxyService blockchainProxyService, params NewFilterInput[] filters)
        {
            BlockchainProxyService = blockchainProxyService ?? throw new ArgumentNullException(nameof(blockchainProxyService));

            if (filters != null)
            {
                Filters.AddRange(filters);
            }
        }

        public string[] ContractAddresses { get; }

        public Action<Exception> FatalErrorCallback { get; set; }

        public Action<uint, BlockRange> BatchProcessedCallback { get; set; }

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
        public uint? BlocksPerBatch { get; set; }

        public IBlockProgressRepository BlockProgressRepository { get; set; }

        public IBlockchainProxyService BlockchainProxyService { get; set; }

        public List<ILogProcessor> Processors { get; private set; } = new List<ILogProcessor>();

        public List<NewFilterInput> Filters { get; private set; } = new List<NewFilterInput>();

        public LogsProcessorBuilder Set(Action<LogsProcessorBuilder> configAction)
        {
            configAction(this);
            return this;
        }

        public LogsProcessorBuilder SetMinimumBlockNumber(ulong minimumBlockNumber)
        {
            MinimumBlockNumber = minimumBlockNumber;
            return this;
        }

        public LogsProcessorBuilder SetBlocksPerBatch(uint blocksPerBatch)
        {
            BlocksPerBatch = blocksPerBatch;
            return this;
        }

        public LogsProcessorBuilder SetMinimumBlockConfirmations(uint minBlockConfirmations)
        {
            MinimumBlockNumber = minBlockConfirmations;
            return this;
        }

        public LogsProcessorBuilder SetBlockProgressRepository(IBlockProgressRepository blockProgressRepository)
        {
            BlockProgressRepository = blockProgressRepository;
            return this;
        }

        public LogsProcessorBuilder OnFatalError(Action<Exception> callBack)
        {
            FatalErrorCallback = callBack;
            return this;
        }

        public LogsProcessorBuilder Add<TEventDto>(Action<IEnumerable<EventLog<TEventDto>>> callBack) where TEventDto : class, new()
        {
            var asyncCallback = new Func<IEnumerable<EventLog<TEventDto>>, Task>(async (events) => await Task.Run(() => callBack(events)).ConfigureAwait(false));
            return Add(asyncCallback);
        }

        public LogsProcessorBuilder Add<TEventDto>(Func<IEnumerable<EventLog<TEventDto>>, Task> callBack) where TEventDto : class, new()
        {
            Processors.Add(new LogProcessor<TEventDto>(callBack));
            return this;
        }

        public LogsProcessorBuilder AddAndQueue<TEventDto>(IQueue queue, Predicate<EventLog<TEventDto>> predicate = null, Func<EventLog<TEventDto>, object> mapper = null) where TEventDto : class, new()
        {
            Processors.Add(new EventLogQueueProcessor<TEventDto>(queue, predicate, mapper));
            return this;
        }

        public LogsProcessorBuilder Add(EventSubscription eventSubscription)
        {
            Processors.Add(eventSubscription);
            return this;
        }

        public LogsProcessorBuilder Add<TEventDto>(EventSubscription<TEventDto> eventSubscription) where TEventDto : class, new()
        {
            Processors.Add(eventSubscription);
            return this;
        }

        public LogsProcessorBuilder Add(ILogProcessor processor)
        {
            Processors.Add(processor);
            return this;
        }

        public LogsProcessorBuilder Add(Action<IEnumerable<FilterLog>> callBack)
        {
            var asyncCallback = new Func<IEnumerable<FilterLog>, Task>(async (events) => await Task.Run(() => callBack(events)).ConfigureAwait(false));
            return Add(asyncCallback);
        }

        public LogsProcessorBuilder Add(Func<IEnumerable<FilterLog>, Task> callBack)
        {
            Processors.Add(new CatchAllLogProcessor(callBack));
            return this;
        }

        public LogsProcessorBuilder AddAndQueue(IQueue queue, Predicate<FilterLog> predicate = null, Func<FilterLog, object> mapper = null)
        {
            Processors.Add(new EventLogQueueProcessor(queue, predicate, mapper));
            return this;
        }

        public LogsProcessorBuilder OnBatchProcessed(Action<uint, BlockRange> batchProcessedCallback)
        {
            BatchProcessedCallback = batchProcessedCallback;
            return this;
        }

        public LogsProcessorBuilder UseBlockProgressRepository(IBlockProgressRepository repo)
        {
            BlockProgressRepository = repo;
            return this;
        }

        public LogsProcessorBuilder UseJsonFileForBlockProgress(string jsonFilePath, bool deleteExistingFile = false)
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
        public LogsProcessorBuilder Filter<TEventDto>() where TEventDto : class, IEventDTO, new()
        {
            var filter = new NewFilterInputBuilder<TEventDto>().Build(ContractAddresses);
            Filters.Add(filter);
            return this;
        }

        public LogsProcessorBuilder Filter<TEventDto>(Action<NewFilterInputBuilder<TEventDto>> configureFilter) where TEventDto : class, IEventDTO, new()
        {
            var filterBuilder = new NewFilterInputBuilder<TEventDto>();
            configureFilter(filterBuilder);
            Filters.Add(filterBuilder.Build(ContractAddresses));
            return this;
        }

        public LogsProcessorBuilder Filter(NewFilterInput filter)
        {
            Filters.Add(filter);
            return this;
        }

        private void SetContractAddressFilter(string[] contractAddresses)
        {
            _contractAddressFilter = new NewFilterInput { Address = contractAddresses };
            Filters.Add(_contractAddressFilter);
        }

        /// <summary>
        /// Register an object to be disposed when the processor disposes
        /// </summary>
        /// <param name="objectToDisposeWithProcessor"></param>
        /// <returns></returns>
        public LogsProcessorBuilder OnProcessorDisposing(IDisposable objectToDisposeWithProcessor)
        {
            _disposalStack.Push(objectToDisposeWithProcessor);
            return this;
        }

        public ILogsProcessor Build()
        {
            if (Processors == null || Processors.Count == 0) throw new ArgumentNullException(nameof(Processors));
            if (BlockchainProxyService == null) throw new ArgumentNullException(nameof(BlockchainProxyService));

            ulong? lastBlockProcessed = (MinimumBlockNumber == null || MinimumBlockNumber == 0) ? null : MinimumBlockNumber - 1;

            BlockProgressRepository = BlockProgressRepository ?? new InMemoryBlockchainProgressRepository(lastBlockProcessed);
            var progressService = new BlockProgressService(BlockchainProxyService, MinimumBlockNumber, BlockProgressRepository, MinimumBlockConfirmations);
            var processor = new BlockchainLogProcessor(BlockchainProxyService, Processors, Filters?.ToArray());
            var batchProcessorService = new BlockchainBatchProcessorService(processor, progressService, BlocksPerBatch, BatchProcessedCallback, FatalErrorCallback);

            batchProcessorService.OnDisposing += disposeHandler;

            return batchProcessorService;

            void disposeHandler(object s, EventArgs src)
            {
                while (_disposalStack.Count > 0)
                {
                    _disposalStack.Pop().Dispose();
                }

                batchProcessorService.OnDisposing -= disposeHandler;
            }
        }

    }
}
