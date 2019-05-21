using Microsoft.Azure.Search.Models;
using Nethereum.BlockchainStore.Search;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Contracts;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public static class EventLogProcessingExtensions
    {
        public static async Task<IEventLogProcessor> AddToSearchIndexAsync<TEventDto>(
            this IEventLogProcessor eventLogProcessor,
            string searchServiceName, 
            string apiKey, 
            string indexName,
            Predicate<EventLog<TEventDto>> predicate = null) where TEventDto : class, new()
        {
            var searchService = eventLogProcessor.CreateAzureSearchServiceAndMarkForDisposal(searchServiceName, apiKey);
            return await eventLogProcessor.AddToSearchIndexAsync(searchService, indexName: indexName, predicate: predicate).ConfigureAwait(false);
        }

        public static async Task<IEventLogProcessor> AddToSearchIndexAsync<TEventDto>(
            this IEventLogProcessor eventLogProcessor, 
            IAzureSearchService azureSearchService, 
            string indexName,
            Predicate<EventLog<TEventDto>> predicate = null) where TEventDto : class, new()
        {
            var indexer = await azureSearchService.CreateEventIndexer<TEventDto>(indexName).ConfigureAwait(false);
            eventLogProcessor.MarkForDisposal(indexer);
            return eventLogProcessor.AddToSearchIndexAsync(indexer, predicate: predicate);
        }

        public static async Task<IEventLogProcessor> AddToSearchIndexAsync<TEventDto, TSearchDocument>(
            this IEventLogProcessor eventLogProcessor,
            IAzureSearchService azureSearchService,
            Index azureIndex,
            IEventToSearchDocumentMapper<TEventDto, TSearchDocument> mapper,
            Predicate<EventLog<TEventDto>> predicate = null) where TEventDto : class, new() where TSearchDocument : class, new()
        {
            var indexer = await azureSearchService.CreateEventIndexer(azureIndex, mapper).ConfigureAwait(false);
            eventLogProcessor.MarkForDisposal(indexer);
            return eventLogProcessor.AddToSearchIndexAsync(indexer, predicate);
        }

        public static async Task<IEventLogProcessor> AddToSearchIndexAsync<TEventDto, TSearchDocument>(
            this IEventLogProcessor eventLogProcessor,
            string searchServiceName,
            string apiKey,
            Index azureIndex,
            IEventToSearchDocumentMapper<TEventDto, TSearchDocument> mapper,
            Predicate<EventLog<TEventDto>> predicate = null) where TEventDto : class, new() where TSearchDocument : class, new()
        {
            var searchService = eventLogProcessor.CreateAzureSearchServiceAndMarkForDisposal(searchServiceName, apiKey);
            return await eventLogProcessor.AddToSearchIndexAsync(searchService, azureIndex, mapper, predicate).ConfigureAwait(false);
        }

        public static async Task<IEventLogProcessor> AddToSearchIndexAsync<TEventDto, TSearchDocument>(
            this IEventLogProcessor eventLogProcessor,
            IAzureSearchService azureSearchService,
            Index azureIndex,
            Func<EventLog<TEventDto>, TSearchDocument> mappingFunc,
            Predicate<EventLog<TEventDto>> predicate = null) where TEventDto : class, new() where TSearchDocument : class, new()
        {

            var indexer = await azureSearchService.CreateEventIndexer(azureIndex, mappingFunc).ConfigureAwait(false);
            eventLogProcessor.MarkForDisposal(indexer);
            return eventLogProcessor.AddToSearchIndexAsync(indexer, predicate);
        }

        public static async Task<IEventLogProcessor> AddToSearchIndexAsync<TEventDto, TSearchDocument>(
            this IEventLogProcessor eventLogProcessor,
            string searchServiceName,
            string apiKey,
            Index azureIndex,
            Func<EventLog<TEventDto>, TSearchDocument> mappingFunc,
            Predicate<EventLog<TEventDto>> predicate = null) where TEventDto : class, new() where TSearchDocument : class, new()
        {
            var searchService = eventLogProcessor.CreateAzureSearchServiceAndMarkForDisposal(searchServiceName, apiKey);
            return await eventLogProcessor.AddToSearchIndexAsync(searchService, azureIndex, mappingFunc, predicate).ConfigureAwait(false);
        }

        public static IEventLogProcessor AddToSearchIndexAsync<TEventDto>(
            this IEventLogProcessor eventLogProcessor,
            IEventIndexer<TEventDto> indexer,
            Predicate<EventLog<TEventDto>> predicate = null) where TEventDto : class, new()
        {
            var processor = new EventIndexProcessor<TEventDto>(indexer, predicate: predicate);
            eventLogProcessor.Subscribe(processor);
            return eventLogProcessor;
        }

        private static AzureSearchService CreateAzureSearchServiceAndMarkForDisposal(this IEventLogProcessor eventLogProcessor,
            string searchServiceName,
            string apiKey)
        {
            var searchService = new AzureSearchService(searchServiceName, apiKey);
            eventLogProcessor.MarkForDisposal(searchService);
            return searchService;
        }

        private static void MarkForDisposal(this IEventLogProcessor eventLogProcessor, IDisposable dependency)
        {
            eventLogProcessor.OnDisposing += disposeHandler;

            void disposeHandler(object s, EventArgs src)
            {
                dependency.Dispose();
                eventLogProcessor.OnDisposing -= disposeHandler;
            }
        }

    }
}
