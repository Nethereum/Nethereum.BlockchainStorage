using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.TransientFaultHandling;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Index = Microsoft.Azure.Search.Models.Index;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureSearchService: IDisposable
    {
        private readonly SearchServiceClient _client;
        private readonly ConcurrentDictionary<string, Index> _azureIndexes;

        public AzureSearchService(string serviceName, string searchApiKey)
        {
            _client = new SearchServiceClient(serviceName, new SearchCredentials(searchApiKey));
            _azureIndexes = new ConcurrentDictionary<string, Index>();
        }

        public async Task<Index> CreateIndexAsync(EventSearchIndexDefinition searchIndex)
        {            
            var index = GetAzureIndex(searchIndex);
            return await _client.Indexes.CreateAsync(index);
        }

        public async Task DeleteIndexAsync(EventSearchIndexDefinition searchIndex)
        {
            var azureIndex = GetAzureIndex(searchIndex);

            if (await _client.Indexes.ExistsAsync(azureIndex.Name))
            {
                await _client.Indexes.DeleteAsync(azureIndex.Name);
            }
        }

        public async Task UpsertAsync<TEvent>(EventSearchIndexDefinition<TEvent> searchIndexDefinition, EventLog<TEvent> log) where TEvent : class, IEventDTO, new()
        {
            var azureIndex = GetAzureIndex(searchIndexDefinition);
            var document = log.ToAzureDocument(searchIndexDefinition);
            await BatchUpdateAsync(azureIndex.Name, new[] {document});
        }

        private Index GetAzureIndex(EventSearchIndexDefinition eventIndex)
        {
            if (_azureIndexes.TryGetValue(eventIndex.IndexName, out var index))
            {
                return index;
            }

            index = eventIndex.ToAzureIndex();
            _azureIndexes.TryAdd(eventIndex.IndexName, index);
            return index;
        }

        private class SearchIndexErrorDetectionStrategy : ITransientErrorDetectionStrategy
        {
            public bool IsTransient(Exception ex)
            {
                return ex is IndexBatchException;
            }
        }

        private async Task BatchUpdateAsync<T>(string indexName, IEnumerable<T> uploadOrMerge, IEnumerable<T> upload = null, IEnumerable<T> delete = null) where T : class
        {
            var indexClient = _client.Indexes.GetClient(indexName);

            var actions = new List<IndexAction<T>>();

            if (uploadOrMerge != null)
            {
                foreach (var item in uploadOrMerge)
                {
                    actions.Add(IndexAction.MergeOrUpload<T>(item));
                }
            }

            if (upload != null)
            {
                foreach (var item in upload)
                {
                    actions.Add(IndexAction.Upload<T>(item));
                }
            }

            if (delete != null)
            {
                foreach (var item in delete)
                {
                    actions.Add(IndexAction.Delete<T>(item));
                }
            }

            var batch = IndexBatch.New(actions);

            var retryStrategy =
                new IncrementalRetryStrategy(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));

            var retryPolicy =
                new RetryPolicy<SearchIndexErrorDetectionStrategy>(retryStrategy);
            //there is a retry policy for the client search now, we might be able to use that instead
            await retryPolicy.ExecuteAsync(async () => await indexClient.Documents.IndexAsync(batch));
        }

        public void Dispose()
        {
            ((IDisposable)_client)?.Dispose();
        }
    }
}
