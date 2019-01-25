using Microsoft.Azure.Search;
using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Index = Microsoft.Azure.Search.Models.Index;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureEventSearchService: IDisposable
    {
        private readonly SearchServiceClient _client;
        private readonly ConcurrentDictionary<string, Index> _azureIndexes;

        public AzureEventSearchService(string serviceName, string searchApiKey)
        {
            _client = new SearchServiceClient(serviceName, new SearchCredentials(searchApiKey));
            _azureIndexes = new ConcurrentDictionary<string, Index>();
        }

        public async Task<IAzureEventIndex<TEvent>> GetOrCreateIndex<TEvent>(EventSearchIndexDefinition<TEvent> searchIndexDefinition) where TEvent : class, new()
        {
            var azureIndex = GetAzureIndex(searchIndexDefinition);

            if (!await _client.Indexes.ExistsAsync(azureIndex.Name))
            {
                azureIndex = await _client.Indexes.CreateAsync(azureIndex);
            }

            return new AzureEventSearchIndex<TEvent>(searchIndexDefinition, azureIndex, _client.Indexes.GetClient(azureIndex.Name));
        }

        public async Task DeleteIndexAsync(SearchIndexDefinition searchIndex)
        {
            var azureIndex = GetAzureIndex(searchIndex);

            if (await _client.Indexes.ExistsAsync(azureIndex.Name))
            {
                await _client.Indexes.DeleteAsync(azureIndex.Name);
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
