using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search.Models;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureEventIndexingProcessor : IDisposable
    {
        private readonly List<ILogProcessor> _logProcessors;
        private readonly List<IIndexer> _indexers;
        private readonly Func<ulong, ulong?, IBlockProgressService> _blockProgressServiceCallBack;
        private readonly IEnumerable<NewFilterInput> _filters;
        private readonly IEventFunctionProcessor _functionProcessor;

        public AzureEventIndexingProcessor(
            string serviceName, 
            string searchApiKey, 
            string blockchainUrl, 
            uint maxBlocksPerBatch = 2, 
            IEnumerable<NewFilterInput> filters = null,
            uint minBlockConfirmations = 0)
            :this(new BlockchainProxyService(blockchainUrl), 
                new AzureSearchService(serviceName, searchApiKey), 
                null,
                null, 
                maxBlocksPerBatch, 
                filters,
                minBlockConfirmations)
        {
        }

        public AzureEventIndexingProcessor(
            IBlockchainProxyService blockchainProxyService, 
            IAzureSearchService searchService, 
            IEventFunctionProcessor functionProcessor,
            Func<ulong, ulong?, IBlockProgressService> blockProgressServiceCallBack = null, 
            uint maxBlocksPerBatch = 2,
            IEnumerable<NewFilterInput> filters = null,
            uint minimumBlockConfirmations = 0)
        {
            SearchService = searchService;
            BlockchainProxyService = blockchainProxyService;
            MaxBlocksPerBatch = maxBlocksPerBatch;
            _filters = filters;
            MinimumBlockConfirmations = minimumBlockConfirmations;
            _blockProgressServiceCallBack = blockProgressServiceCallBack;
            _logProcessors = new List<ILogProcessor>();
            _indexers = new List<IIndexer>();
            _functionProcessor = functionProcessor ?? new EventFunctionProcessor(BlockchainProxyService);
        }

        public IAzureSearchService SearchService {get;}
        public IBlockchainProxyService BlockchainProxyService { get; }
        public uint MaxBlocksPerBatch { get; }
        public uint MinimumBlockConfirmations { get; }

        public IReadOnlyList<IIndexer> Indexers => _indexers.AsReadOnly();
        
        public async Task<FunctionIndexTransactionHandler<TFunctionMessage>> CreateFunctionHandlerAsync<TFunctionMessage>(
            string indexName = null)
            where TFunctionMessage : FunctionMessage, new()
        {
            var functionIndexer = await SearchService.CreateFunctionIndexer<TFunctionMessage>(indexName);
            var functionHandler = new FunctionIndexTransactionHandler<TFunctionMessage>(functionIndexer);
            return functionHandler;
        }

        public async Task<IEventIndexProcessor<TEvent>> AddAsync<TEvent>(
            string indexName = null,
            IEnumerable<ITransactionHandler> functionHandlers = null) where TEvent : class, new()
        {

            var indexer = await SearchService.CreateEventIndexer<TEvent>(indexName);
            _indexers.Add(indexer);

            return CreateProcessor(functionHandlers, indexer);
        }

        public async Task<IEventIndexProcessor<TEvent>> AddAsync<TEvent, TSearchDocument>(
            Index index, Func<EventLog<TEvent>, TSearchDocument> mappingFunc,
            IEnumerable<ITransactionHandler> functionHandlers = null) where TEvent : class, new() where TSearchDocument : class, new()
        {

            var indexer = await SearchService.CreateEventIndexer(index, mappingFunc);
            _indexers.Add(indexer);
            
            return CreateProcessor(functionHandlers, indexer);
        }

        public async Task<IEventIndexProcessor<TEvent>> AddAsync<TEvent, TSearchDocument>(
            Index index, IEventToSearchDocumentMapper<TEvent, TSearchDocument> mapper,
            IEnumerable<ITransactionHandler> functionHandlers = null) where TEvent : class, new() where TSearchDocument : class, new()
        {

            var indexer = await SearchService.CreateEventIndexer(index, mapper);
            _indexers.Add(indexer);
            
            return CreateProcessor(functionHandlers, indexer);
        }

        public async Task<ulong> ProcessAsync(ulong from, ulong? to = null, CancellationTokenSource ctx = null, Action<uint, BlockRange> rangeProcessedCallback = null)
        {
            if(!_logProcessors.Any()) throw new InvalidOperationException("No events to capture - use AddEventAsync to add listeners for indexable events");

            var logProcessor = new BlockchainLogProcessor(
                BlockchainProxyService,
                _logProcessors,
                _filters);

            IBlockProgressService progressService = CreateProgressService(from, to);

            var batchProcessorService = new BlockchainBatchProcessorService(
                logProcessor, progressService, maxNumberOfBlocksPerBatch: MaxBlocksPerBatch);

            if (to != null)
            {
                return await ProcessRange(ctx, rangeProcessedCallback, batchProcessorService);
            }

            return await batchProcessorService.ProcessContinuallyAsync(ctx?.Token ?? new CancellationToken(), rangeProcessedCallback);
            
        }

        private static async Task<ulong> ProcessRange(CancellationTokenSource ctx, Action<uint, BlockRange> rangeProcessedCallback, BlockchainBatchProcessorService batchProcessorService)
        {
            uint blockRangesProcessed = 0;
            ulong blocksProcessed = 0;

            BlockRange? lastBlockRangeProcessed;
            do
            {
                lastBlockRangeProcessed = await batchProcessorService.ProcessLatestBlocksAsync(ctx?.Token ?? new CancellationToken());

                if (lastBlockRangeProcessed != null)
                {
                    blockRangesProcessed++;
                    blocksProcessed += lastBlockRangeProcessed.Value.BlockCount;
                    rangeProcessedCallback?.Invoke(blockRangesProcessed, lastBlockRangeProcessed.Value);
                }

            } while (lastBlockRangeProcessed != null);

            return blocksProcessed;
        }


        private IBlockProgressService CreateProgressService(ulong from, ulong? to)
        {
            if (_blockProgressServiceCallBack != null) return _blockProgressServiceCallBack.Invoke(from, to);

            var progressRepository =
                new JsonBlockProgressRepository(PathToJsonProgressFile());

            IBlockProgressService progressService = null;
            if (to == null)
            {
                progressService = new BlockProgressService(BlockchainProxyService, from, progressRepository, MinimumBlockConfirmations);
            }
            else
            {
                progressService = new StaticBlockRangeProgressService(from, to.Value, progressRepository);
            }

            return progressService;
        }

        private string PathToJsonProgressFile()
        {
            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), $"{this.GetType().Name}_Progress.json");
            return progressFileNameAndPath;
        }

        public Task ClearProgress()
        {
            var jsonFile = PathToJsonProgressFile();
            if (File.Exists(jsonFile)) File.Delete(jsonFile);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            foreach (var processor in _logProcessors)
            {
                if (processor is IDisposable d)
                {
                    d.Dispose();
                }
            }

            SearchService?.Dispose();
        }

        private IEventIndexProcessor<TEvent> CreateProcessor<TEvent>(IEnumerable<ITransactionHandler> functionHandlers, IAzureEventIndexer<TEvent> indexer) where TEvent : class, new()
        {
            var processor = new EventIndexProcessor<TEvent>(indexer, _functionProcessor);
            _logProcessors.Add(processor);

            if (functionHandlers != null)
            {
                foreach (var functionHandler in functionHandlers)
                {
                    _functionProcessor.AddHandler<TEvent>(functionHandler);
                }
            }

            return processor;
        }
    }
}
