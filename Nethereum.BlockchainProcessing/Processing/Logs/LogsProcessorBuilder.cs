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
    public class LogsProcessorBuilder : ILogsProcessorBuilder
    {
        private NewFilterInput _contractAddressFilter;

        private Stack<IDisposable> _disposalStack = new Stack<IDisposable>();

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
            this(web3.Eth, contractAddresses: null)
        { }
        public LogsProcessorBuilder(Web3.IWeb3 web3, string contractAddress) :
            this(web3.Eth, new[] { contractAddress })
        { }

        public LogsProcessorBuilder(Web3.IWeb3 web3, string[] contractAddresses) :
            this(web3.Eth, contractAddresses)
        { }

        public LogsProcessorBuilder(IEthApiContractService eth, string contractAddress) :
            this(eth, string.IsNullOrEmpty(contractAddress) ? null : new[] { contractAddress })
        { }

        public LogsProcessorBuilder(IEthApiContractService eth, string[] contractAddresses)
        {
            Eth = eth ?? throw new ArgumentNullException(nameof(eth));
            ContractAddresses = contractAddresses;

            if (ContractAddresses != null)
            {
                SetContractAddressFilter(ContractAddresses);
            }
        }

        public LogsProcessorBuilder(IEthApiContractService eth, params NewFilterInput[] filters)
        {
            Eth = eth ?? throw new ArgumentNullException(nameof(eth));

            if (filters != null)
            {
                Filters.AddRange(filters);
            }
        }

        public LogsProcessorBuilder(IEthApiContractService eth)
        {
            Eth = eth ?? throw new ArgumentNullException(nameof(eth));
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

        public List<ILogProcessor> Processors { get; private set; } = new List<ILogProcessor>();

        public List<NewFilterInput> Filters { get; private set; } = new List<NewFilterInput>();
        public IEthApiContractService Eth { get; set; }

        public ILogsProcessorBuilder Set(Action<ILogsProcessorBuilder> configAction)
        {
            configAction(this);
            return this;
        }

        public ILogsProcessorBuilder SetMinimumBlockNumber(ulong minimumBlockNumber)
        {
            MinimumBlockNumber = minimumBlockNumber;
            return this;
        }

        public ILogsProcessorBuilder SetBlocksPerBatch(uint blocksPerBatch)
        {
            BlocksPerBatch = blocksPerBatch;
            return this;
        }

        public ILogsProcessorBuilder SetMinimumBlockConfirmations(uint minBlockConfirmations)
        {
            MinimumBlockNumber = minBlockConfirmations;
            return this;
        }

        public ILogsProcessorBuilder SetBlockProgressRepository(IBlockProgressRepository blockProgressRepository)
        {
            BlockProgressRepository = blockProgressRepository;
            return this;
        }

        public ILogsProcessorBuilder OnFatalError(Action<Exception> callBack)
        {
            FatalErrorCallback = callBack;
            return this;
        }

        public ILogsProcessorBuilder Add<TEventDto>(Action<IEnumerable<EventLog<TEventDto>>> callBack) where TEventDto : class, new()
        {
            return Add(callBack.ToFunc());
        }

        public ILogsProcessorBuilder Add<TEventDto>(Func<IEnumerable<EventLog<TEventDto>>, Task> callBack) where TEventDto : class, new()
        {
            Processors.Add(new LogProcessor<TEventDto>(callBack));
            return this;
        }

        public ILogsProcessorBuilder AddToQueue<TEventDto>(IQueue queue, Predicate<EventLog<TEventDto>> predicate = null, Func<EventLog<TEventDto>, object> mapper = null) where TEventDto : class, new()
        {
            Processors.Add(new EventLogQueueProcessor<TEventDto>(queue, predicate, mapper));
            return this;
        }

        public ILogsProcessorBuilder Add(EventSubscription eventSubscription)
        {
            Processors.Add(eventSubscription);
            return this;
        }

        public ILogsProcessorBuilder Add<TEventDto>(EventSubscription<TEventDto> eventSubscription) where TEventDto : class, new()
        {
            Processors.Add(eventSubscription);
            return this;
        }

        public ILogsProcessorBuilder Add(ILogProcessor processor)
        {
            Processors.Add(processor);
            return this;
        }

        public ILogsProcessorBuilder Add(Action<IEnumerable<FilterLog>> callBack)
        {
            return Add(callBack.ToFunc());
        }

        public ILogsProcessorBuilder Add(Func<IEnumerable<FilterLog>, Task> callBack)
        {
            Processors.Add(new CatchAllFilterLogProcessor(callBack));
            return this;
        }

        public ILogsProcessorBuilder Add(Predicate<FilterLog> isItForMe, Action<IEnumerable<FilterLog>> action)
        {
            return Add(isItForMe, action.ToFunc());
        }

        public ILogsProcessorBuilder Add(Predicate<FilterLog> isItForMe, Func<IEnumerable<FilterLog>, Task> func)
        {
            Processors.Add(new FilterLogProcessor(isItForMe, func));
            return this;
        }

        public ILogsProcessorBuilder AddToQueue(IQueue queue, Predicate<FilterLog> predicate = null, Func<FilterLog, object> mapper = null)
        {
            Processors.Add(new EventLogQueueProcessor(queue, predicate, mapper));
            return this;
        }

        public ILogsProcessorBuilder OnBatchProcessed(Action<uint, BlockRange> batchProcessedCallback)
        {
            BatchProcessedCallback = batchProcessedCallback;
            return this;
        }

        public ILogsProcessorBuilder OnBatchProcessed(Action batchProcessedCallback)
        {
            BatchProcessedCallback = (c, r) => { batchProcessedCallback();};
            return this;
        }

        public ILogsProcessorBuilder UseBlockProgressRepository(IBlockProgressRepository repo)
        {
            BlockProgressRepository = repo;
            return this;
        }

        public ILogsProcessorBuilder UseJsonFileForBlockProgress(string jsonFilePath, bool deleteExistingFile = false)
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
        public ILogsProcessorBuilder Filter<TEventDto>() where TEventDto : class, IEventDTO, new()
        {
            var filter = new NewFilterInputBuilder<TEventDto>().Build(ContractAddresses);

            AddOrReplaceContractAddressFilter(filter);

            return Filter(filter);
        }

        public ILogsProcessorBuilder Filter<TEventDto>(Action<NewFilterInputBuilder<TEventDto>> configureFilter) where TEventDto : class, IEventDTO, new()
        {
            var filterBuilder = new NewFilterInputBuilder<TEventDto>();
            configureFilter(filterBuilder);
            var filter = filterBuilder.Build(ContractAddresses);
            AddOrReplaceContractAddressFilter(filter);
            return Filter(filter);
        }

        public ILogsProcessorBuilder Filter(NewFilterInput filter)
        {
            Filters.Add(filter);
            return this;
        }

        private void AddOrReplaceContractAddressFilter(NewFilterInput filter)
        {
            //if we only have an event agnostic contract address filter in place - replace it with an event specific filter
            if (Filters.Count == 1 && _contractAddressFilter != null && _contractAddressFilter.Topics?[0] == null)
            {
                Filters.Clear();
                _contractAddressFilter = filter;
            }
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
        public ILogsProcessorBuilder DisposeOnProcessorDisposing(IDisposable objectToDisposeWithProcessor)
        {
            _disposalStack.Push(objectToDisposeWithProcessor);
            return this;
        }

        public ILogsProcessor Build()
        {
            if (Processors == null || Processors.Count == 0) throw new ArgumentNullException(nameof(Processors));
            if (Eth == null) throw new ArgumentNullException(nameof(Eth));

            ulong? lastBlockProcessed = (MinimumBlockNumber == null || MinimumBlockNumber == 0) ? null : MinimumBlockNumber - 1;

            BlockProgressRepository = BlockProgressRepository ?? new InMemoryBlockchainProgressRepository(lastBlockProcessed);
            var progressService = new BlockProgressService(Eth.Blocks, MinimumBlockNumber, BlockProgressRepository, MinimumBlockConfirmations);
            var processor = new BlockRangeLogsProcessor(Eth.Filters.GetLogs, Processors, Filters?.ToArray());
            var batchProcessorService = new LogsProcessor(processor, progressService, BlocksPerBatch, BatchProcessedCallback, FatalErrorCallback);

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
