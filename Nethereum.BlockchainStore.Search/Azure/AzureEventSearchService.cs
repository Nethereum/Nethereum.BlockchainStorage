using Microsoft.Azure.Search;
using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Index = Microsoft.Azure.Search.Models.Index;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureEventSearchService : IAzureEventSearchService
    {
        private readonly SearchServiceClient _client;
        private readonly ConcurrentDictionary<string, Index> _azureIndexes;

        public AzureEventSearchService(string serviceName, string searchApiKey)
        {
            _client = new SearchServiceClient(serviceName, new SearchCredentials(searchApiKey));
            _azureIndexes = new ConcurrentDictionary<string, Index>();
        }

        public async Task<IAzureEventSearchIndex<TEvent>> GetOrCreateIndex<TEvent>(string indexName = null) where TEvent : class
        {
            return await GetOrCreateIndex<TEvent>(new EventSearchIndexDefinition<TEvent>(indexName));
        }

        public async Task<IAzureEventSearchIndex<TEvent>> GetOrCreateIndex<TEvent>(EventSearchIndexDefinition<TEvent> searchIndexDefinition) where TEvent : class
        {
            var azureIndex = GetAzureIndex(searchIndexDefinition);

            if (!await _client.Indexes.ExistsAsync(azureIndex.Name))
            {
                azureIndex = await _client.Indexes.CreateAsync(azureIndex);
            }

            return new AzureEventSearchSearchIndex<TEvent>(searchIndexDefinition, azureIndex, _client.Indexes.GetClient(azureIndex.Name));
        }

        public Task DeleteIndexAsync(SearchIndexDefinition searchIndex) =>
            DeleteIndexAsync(GetAzureIndex(searchIndex).Name);

        public async Task DeleteIndexAsync(string indexName)
        {
            if (await _client.Indexes.ExistsAsync(indexName))
            {
                await _client.Indexes.DeleteAsync(indexName);
            }
        }

        private Index GetAzureIndex(SearchIndexDefinition eventIndex)
        {
            if (_azureIndexes.TryGetValue(eventIndex.IndexName, out var index))
            {
                return index;
            }

            index = eventIndex.ToAzureIndex();
            _azureIndexes.TryAdd(eventIndex.IndexName, index);
            return index;
        }

        public void Dispose()
        {
            ((IDisposable)_client)?.Dispose();
        }
    }
}
