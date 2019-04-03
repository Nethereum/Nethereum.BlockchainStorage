using Microsoft.Azure.Search.Models;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureEventIndexingProcessor : EventIndexingProcessor
    {
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
            uint minimumBlockConfirmations = 0):
            base(blockchainProxyService, searchService, functionProcessor, blockProgressServiceCallBack, maxBlocksPerBatch, filters, minimumBlockConfirmations )
        {
            AzureSearchService = searchService;
        }

        public IAzureSearchService AzureSearchService {get;}
  
        public async Task<FunctionIndexTransactionHandler<TFunctionMessage>> CreateFunctionHandlerAsync<TFunctionMessage, TSearchDocument>(
            Index index, Func<FunctionCall<TFunctionMessage>, TSearchDocument> mappingFunc)
            where TFunctionMessage : FunctionMessage, new()
            where TSearchDocument : class, new()
        {
            var functionIndexer = await AzureSearchService.CreateFunctionIndexer<TFunctionMessage, TSearchDocument>(index, mappingFunc);
            var functionHandler = new FunctionIndexTransactionHandler<TFunctionMessage>(functionIndexer);
            return functionHandler;
        }

        public async Task<FunctionIndexTransactionHandler<TFunctionMessage>> CreateFunctionHandlerAsync<TFunctionMessage, TSearchDocument>(
            Index index, IFunctionMessageToSearchDocumentMapper<TFunctionMessage, TSearchDocument> mapper)
            where TFunctionMessage : FunctionMessage, new()
            where TSearchDocument : class, new()
        {
            var functionIndexer = await AzureSearchService.CreateFunctionIndexer<TFunctionMessage, TSearchDocument>(index, mapper);
            var functionHandler = new FunctionIndexTransactionHandler<TFunctionMessage>(functionIndexer);
            return functionHandler;
        }

        public async Task<IEventIndexProcessor<TEvent>> AddAsync<TEvent, TSearchDocument>(
            Index index, Func<EventLog<TEvent>, TSearchDocument> mappingFunc,
            IEnumerable<ITransactionHandler> functionHandlers = null) where TEvent : class, new() where TSearchDocument : class, new()
        {

            var indexer = await AzureSearchService.CreateEventIndexer(index, mappingFunc);
            _indexers.Add(indexer);
            
            return CreateProcessor(functionHandlers, indexer);
        }

        public async Task<IEventIndexProcessor<TEvent>> AddAsync<TEvent, TSearchDocument>(
            Index index, IEventToSearchDocumentMapper<TEvent, TSearchDocument> mapper,
            IEnumerable<ITransactionHandler> functionHandlers = null) where TEvent : class, new() where TSearchDocument : class, new()
        {

            var indexer = await AzureSearchService.CreateEventIndexer(index, mapper);
            _indexers.Add(indexer);
            
            return CreateProcessor(functionHandlers, indexer);
        }

    }
}
