using Nest;
using Nethereum.Contracts;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.ElasticSearch
{
    public class ElasticSearchService : IElasticSearchService
    {
        private readonly IElasticClient _elasticClient;

        public ElasticSearchService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<long> CountDocumentsAsync(string indexName)
        {
            var countResponse = await _elasticClient.CountAsync<object>(new CountRequest(indexName));
            return countResponse.Count;
        }

        public async Task<IEventIndexer<TEvent>> CreateEventIndexer<TEvent>(EventIndexDefinition<TEvent> searchIndexDefinition) where TEvent : class
        {
            await CreateIfNotExists(searchIndexDefinition);

            var indexer =
                new ElasticEventIndexer<TEvent>(_elasticClient, searchIndexDefinition);

            return indexer;
        }

        public async Task<IEventIndexer<TEvent>> CreateEventIndexer<TEvent>(string indexName = null, bool addPresetEventLogFields = true) where TEvent : class
        {
            var eventIndexDefinition = new EventIndexDefinition<TEvent>(indexName, addPresetEventLogFields);
            return await CreateEventIndexer(eventIndexDefinition);
        }

        private async Task CreateIfNotExists(IndexDefinition searchIndexDefinition)
        {
            var indexName = searchIndexDefinition.ElasticIndexName();
            var existsResponse = await _elasticClient.IndexExistsAsync(new IndexExistsRequest(indexName));
            if (!existsResponse.Exists)
            {
                var mappings = searchIndexDefinition.CreateElasticMappings();
                var createResponse = await _elasticClient.CreateIndexAsync(new CreateIndexRequest(searchIndexDefinition.ElasticIndexName())
                {
                    Mappings = mappings
                });

                if (!createResponse.IsValid)
                {
                    throw new Exception("Error creating index.", createResponse.OriginalException);
                }
            }
        }

        private async Task CreateIfNotExists(string indexName)
        {
            indexName = indexName.ToElasticName();
            var existsResponse = await _elasticClient.IndexExistsAsync(new IndexExistsRequest(indexName));
            if (!existsResponse.Exists)
            {
                var createResponse = await _elasticClient.CreateIndexAsync(new CreateIndexRequest(indexName));

                if (!createResponse.IsValid)
                {
                    throw new Exception("Error creating index.", createResponse.OriginalException);
                }
            }
        }

        public async Task<IFunctionIndexer<TFunctionMessage>> CreateFunctionIndexer<TFunctionMessage>(FunctionIndexDefinition<TFunctionMessage> searchIndexDefinition) where TFunctionMessage : FunctionMessage, new()
        {
            await CreateIfNotExists(searchIndexDefinition);

            var indexer =
                new ElasticFunctionIndexer<TFunctionMessage>(_elasticClient, searchIndexDefinition);

            return indexer;
        }

        public async Task<IFunctionIndexer<TFunctionMessage>> CreateFunctionIndexer<TFunctionMessage>(string indexName = null, bool addPresetEventLogFields = true) where TFunctionMessage : FunctionMessage, new()
        {
            var searchIndexDefinition =
                new FunctionIndexDefinition<TFunctionMessage>(indexName, addPresetEventLogFields);

            return await CreateFunctionIndexer(searchIndexDefinition);
        }

        public Task DeleteIndexAsync(IndexDefinition searchIndex) => DeleteIndexAsync(searchIndex.ElasticIndexName());

        public async Task DeleteIndexAsync(string indexName)
        {
            var existsResponse = await _elasticClient.IndexExistsAsync(new IndexExistsRequest(indexName));
            if (existsResponse.Exists)
            {
                await _elasticClient.DeleteIndexAsync(indexName);
            }
        }

        public void Dispose(){}

        public async Task<IEventIndexer<TEvent>> CreateEventIndexer<TEvent, TSearchDocument>(string indexName, IEventToSearchDocumentMapper<TEvent, TSearchDocument> mapper)
            where TEvent : class
            where TSearchDocument : class, IHasId, new()
        {
            await CreateIfNotExists(indexName);

            var indexer =
                new ElasticEventIndexer<TEvent, TSearchDocument>(_elasticClient, indexName, mapper);

            return indexer;
        }

        public async Task<IEventIndexer<TEvent>> CreateEventIndexer<TEvent, TSearchDocument>(string indexName, Func<EventLog<TEvent>, TSearchDocument> mappingFunc)
            where TEvent : class
            where TSearchDocument : class, IHasId, new()
        {
            var mapper = new EventToSearchDocumentMapper<TEvent, TSearchDocument>(mappingFunc);

            return await CreateEventIndexer(indexName, mapper);
        }

        public async Task<IFunctionIndexer<TFunctionMessage>> CreateFunctionIndexer<TFunctionMessage, TSearchDocument>(string indexName, IFunctionMessageToSearchDocumentMapper<TFunctionMessage, TSearchDocument> mapper)
            where TFunctionMessage : FunctionMessage, new()
            where TSearchDocument : class, IHasId, new()
        {
            await CreateIfNotExists(indexName);

            var indexer =
                new ElasticFunctionIndexer<TFunctionMessage, TSearchDocument>(_elasticClient, indexName, mapper);

            return indexer;
        }

        public async Task<IFunctionIndexer<TFunctionMessage>> CreateFunctionIndexer<TFunctionMessage, TSearchDocument>(string indexName, Func<FunctionCall<TFunctionMessage>, TSearchDocument> mapperFunc)
            where TFunctionMessage : FunctionMessage, new()
            where TSearchDocument : class, IHasId, new()
        {
            var mapper = new FunctionMessageToSearchDocumentMapper<TFunctionMessage, TSearchDocument>(mapperFunc);
            return await CreateFunctionIndexer(indexName, mapper);
        }
    }
}
