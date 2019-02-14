using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureEventIndexingProcessor : IDisposable
    {
        private readonly List<ILogProcessor> _logProcessors;
        private readonly List<IEventIndexer> _indexers;
        private readonly Func<ulong, ulong?, IBlockProgressService> _blockProgressServiceCallBack;
        private readonly IEnumerable<NewFilterInput> _filters;

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
                maxBlocksPerBatch, 
                filters,
                minBlockConfirmations)
        {
        }

        public AzureEventIndexingProcessor(
            IBlockchainProxyService blockchainProxyService, 
            IAzureEventAndFunctionIndexingService searchService, 
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
            _indexers = new List<IEventIndexer>();
        }

        public IAzureEventAndFunctionIndexingService SearchService {get;}
        public IBlockchainProxyService BlockchainProxyService { get; }
        public uint MaxBlocksPerBatch { get; }
        public uint MinimumBlockConfirmations { get; }

        public IReadOnlyList<IEventIndexer> Indexers => _indexers.AsReadOnly();
        
        /// <summary>
        /// Creates a function processor containing a function indexer
        /// This processor can then be inserted into event processors to index related functions
        /// </summary>
        /// <typeparam name="TFunctionMessage"></typeparam>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public async Task<IEventFunctionProcessor<TFunctionMessage>> CreateFunctionProcessorAsync<TFunctionMessage>(
            string indexName = null)
            where TFunctionMessage : FunctionMessage, new()
        {
            var functionIndexer = await SearchService.GetOrCreateFunctionIndexer<TFunctionMessage>(indexName);

            var functionHandler = new FunctionIndexTransactionHandler<TFunctionMessage>(functionIndexer);

            var functionProcessor =
                new EventFunctionProcessor<TFunctionMessage>(BlockchainProxyService, functionHandler);

            return functionProcessor;
        }

        public async Task<EventIndexProcessor<TEvent>> AddIndexer<TEvent>(
            string indexName = null, 
            IEnumerable<IEventFunctionProcessor> functionProcessors = null) where TEvent : class, new()
        {

            var index = await SearchService.GetOrCreateEventIndexer<TEvent>(indexName);
            _indexers.Add(index);
            
            var processor = new EventIndexProcessor<TEvent>(index, functionProcessors: functionProcessors);
            _logProcessors.Add(processor);
            return processor;
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
    }
}
