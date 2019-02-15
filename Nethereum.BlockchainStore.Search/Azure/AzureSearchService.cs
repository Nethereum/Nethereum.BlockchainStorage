using Microsoft.Azure.Search;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Index = Microsoft.Azure.Search.Models.Index;
using Microsoft.Azure.Search.Models;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureSearchService : IAzureSearchService
    {
        private readonly SearchServiceClient _client;
        private readonly ConcurrentDictionary<string, Index> _azureIndexes;

        public AzureSearchService(string serviceName, string searchApiKey)
        {
            _client = new SearchServiceClient(serviceName, new SearchCredentials(searchApiKey));
            _azureIndexes = new ConcurrentDictionary<string, Index>();
        }

        public async Task<IAzureEventIndexer<TEvent>> GetOrCreateEventIndexer<TEvent>(string indexName = null, bool addPresetEventLogFields = true) where TEvent : class
        {
            return await GetOrCreateEventIndexer(new EventIndexDefinition<TEvent>(indexName, addPresetEventLogFields));
        }

        public async Task<IAzureEventIndexer<TEvent>> GetOrCreateEventIndexer<TEvent>(EventIndexDefinition<TEvent> searchIndexDefinition) where TEvent : class
        {
            var azureIndex = await GetOrCreateAzureIndex(searchIndexDefinition);
            return new AzureEventIndexer<TEvent>(searchIndexDefinition, azureIndex, _client.Indexes.GetClient(azureIndex.Name));
        }

        public async Task<IAzureFunctionIndexer<TFunctionMessage>> GetOrCreateFunctionIndexer<TFunctionMessage>(string indexName = null,bool addPresetEventLogFields = true) 
            where TFunctionMessage : FunctionMessage, new()
        {
            return await GetOrCreateFunctionIndexer(new FunctionIndexDefinition<TFunctionMessage>(indexName, addPresetEventLogFields));
        }

        public async Task<IAzureFunctionIndexer<TFunctionMessage>> GetOrCreateFunctionIndexer<TFunctionMessage>(
            FunctionIndexDefinition<TFunctionMessage> searchIndexDefinition) 
            where TFunctionMessage : FunctionMessage, new()
        {
            var azureIndex = await GetOrCreateAzureIndex(searchIndexDefinition);
            return new AzureFunctionIndexer<TFunctionMessage>(searchIndexDefinition, azureIndex, _client.Indexes.GetClient(azureIndex.Name));
        }

        public Task DeleteIndexAsync(IndexDefinition searchIndex) =>
            DeleteIndexAsync(GetAzureIndex(searchIndex).Name);

        public async Task DeleteIndexAsync(string indexName)
        {
            if (await _client.Indexes.ExistsAsync(indexName))
            {
                await _client.Indexes.DeleteAsync(indexName);
            }
        }

        public void Dispose()
        {
            ((IDisposable)_client)?.Dispose();
        }

        private Index GetAzureIndex(IndexDefinition eventIndex)
        {
            if (_azureIndexes.TryGetValue(eventIndex.IndexName, out var index))
            {
                return index;
            }

            index = eventIndex.ToAzureIndex();
            _azureIndexes.TryAdd(eventIndex.IndexName, index);
            return index;
        }

        protected virtual async Task<Index> GetOrCreateAzureIndex(IndexDefinition indexDefinition)
        {
            var azureIndex = GetAzureIndex(indexDefinition);

            if (!await _client.Indexes.ExistsAsync(azureIndex.Name))
            {
                azureIndex = await _client.Indexes.CreateAsync(azureIndex);
            }

            return azureIndex;
        }

        public async Task<long> CountDocumentsAsync(string indexName)
        {
            using(var client = _client.Indexes.GetClient(indexName))
            {
                return await client.Documents.CountAsync();
            }
        }
    }
}
