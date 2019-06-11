using Microsoft.Azure.Search.Models;
using Nethereum.BlockchainStore.Search;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Contracts;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public static class LogsProcessingExtensions
    {
        public static async Task<ILogsProcessorBuilder> AddToSearchIndexAsync<TEventDto>(
            this ILogsProcessorBuilder processorBuilder,
            string searchServiceName,
            string apiKey,
            string indexName,
            Predicate<EventLog<TEventDto>> predicate = null) where TEventDto : class, new()
        {
            var searchService = processorBuilder.CreateAzureSearchServiceAndMarkForDisposal(searchServiceName, apiKey);
            return await processorBuilder.AddToSearchIndexAsync(searchService, indexName: indexName, predicate: predicate).ConfigureAwait(false);
        }

        public static async Task<ILogsProcessorBuilder> AddToSearchIndexAsync<TEventDto>(
            this ILogsProcessorBuilder processorBuilder,
            IAzureSearchService azureSearchService,
            string indexName,
            Predicate<EventLog<TEventDto>> predicate = null) where TEventDto : class, new()
        {
            var indexer = await azureSearchService.CreateEventIndexer<TEventDto>(indexName).ConfigureAwait(false);
            processorBuilder.DisposeOnProcessorDisposing(indexer);
            return processorBuilder.AddToSearchIndex(indexer, predicate: predicate);
        }

        public static async Task<ILogsProcessorBuilder> AddToSearchIndexAsync<TEventDto, TSearchDocument>(
            this ILogsProcessorBuilder processorBuilder,
            IAzureSearchService azureSearchService,
            Index azureIndex,
            IEventToSearchDocumentMapper<TEventDto, TSearchDocument> mapper,
            Predicate<EventLog<TEventDto>> predicate = null) where TEventDto : class, new() where TSearchDocument : class, new()
        {
            var indexer = await azureSearchService.CreateEventIndexer(azureIndex, mapper).ConfigureAwait(false);
            processorBuilder.DisposeOnProcessorDisposing(indexer);
            return processorBuilder.AddToSearchIndex(indexer, predicate);
        }

        public static async Task<ILogsProcessorBuilder> AddToSearchIndexAsync<TEventDto, TSearchDocument>(
            this ILogsProcessorBuilder processorBuilder,
            string searchServiceName,
            string apiKey,
            Index azureIndex,
            IEventToSearchDocumentMapper<TEventDto, TSearchDocument> mapper,
            Predicate<EventLog<TEventDto>> predicate = null) where TEventDto : class, new() where TSearchDocument : class, new()
        {
            var searchService = processorBuilder.CreateAzureSearchServiceAndMarkForDisposal(searchServiceName, apiKey);
            return await processorBuilder.AddToSearchIndexAsync(searchService, azureIndex, mapper, predicate).ConfigureAwait(false);
        }

        public static async Task<ILogsProcessorBuilder> AddToSearchIndexAsync<TEventDto, TSearchDocument>(
            this ILogsProcessorBuilder processorBuilder,
            IAzureSearchService azureSearchService,
            Index azureIndex,
            Func<EventLog<TEventDto>, TSearchDocument> mappingFunc,
            Predicate<EventLog<TEventDto>> predicate = null) where TEventDto : class, new() where TSearchDocument : class, new()
        {

            var indexer = await azureSearchService.CreateEventIndexer(azureIndex, mappingFunc).ConfigureAwait(false);
            processorBuilder.DisposeOnProcessorDisposing(indexer);
            return processorBuilder.AddToSearchIndex(indexer, predicate);
        }

        public static async Task<ILogsProcessorBuilder> AddToSearchIndexAsync<TEventDto, TSearchDocument>(
            this ILogsProcessorBuilder processorBuilder,
            string searchServiceName,
            string apiKey,
            Index azureIndex,
            Func<EventLog<TEventDto>, TSearchDocument> mappingFunc,
            Predicate<EventLog<TEventDto>> predicate = null) where TEventDto : class, new() where TSearchDocument : class, new()
        {
            var searchService = processorBuilder.CreateAzureSearchServiceAndMarkForDisposal(searchServiceName, apiKey);
            return await processorBuilder.AddToSearchIndexAsync(searchService, azureIndex, mappingFunc, predicate).ConfigureAwait(false);
        }

        public static ILogsProcessorBuilder AddToSearchIndex<TEventDto>(
            this ILogsProcessorBuilder processorBuilder,
            IEventIndexer<TEventDto> indexer,
            Predicate<EventLog<TEventDto>> predicate = null) where TEventDto : class, new()
        {
            var processor = new EventIndexProcessor<TEventDto>(indexer, predicate: predicate);
            processorBuilder.Add(processor);
            return processorBuilder;
        }

        private static AzureSearchService CreateAzureSearchServiceAndMarkForDisposal(this ILogsProcessorBuilder processorBuilder,
            string searchServiceName,
            string apiKey)
        {
            var searchService = new AzureSearchService(searchServiceName, apiKey);
            processorBuilder.DisposeOnProcessorDisposing(searchService);
            return searchService;
        }

    }
}
