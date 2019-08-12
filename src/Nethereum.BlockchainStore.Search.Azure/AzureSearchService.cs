using Microsoft.Azure.Search;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Index = Microsoft.Azure.Search.Models.Index;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureSearchService : IAzureSearchService, IDisposable
    {
        private readonly SearchServiceClient _client;
        private readonly ConcurrentDictionary<string, Index> _indexes;
        private readonly Dictionary<string, ISearchIndexClient> _clients;
        private readonly List<IDisposable> _indexers;

        public AzureSearchService(string serviceName, string searchApiKey)
        {
            _client = new SearchServiceClient(serviceName, new SearchCredentials(searchApiKey));
            _indexes = new ConcurrentDictionary<string, Index>();
            _clients = new Dictionary<string, ISearchIndexClient>();
            _indexers = new List<IDisposable>();
        }

        public async Task<long> CountDocumentsAsync(string indexName)
        {
            var client = GetOrCreateIndexClient(indexName);
            return await client.Documents.CountAsync().ConfigureAwait(false);
            
        }

        public ISearchIndexClient GetOrCreateIndexClient(string indexName)
        {
            if (_clients.ContainsKey(indexName))
            {
                return _clients[indexName];
            }

            var client = _client.Indexes.GetClient(indexName);
            _clients.Add(indexName, client);
            return client;
        }

        public Task<bool> IndexExistsAsync(string indexName)
            => _client.Indexes.ExistsAsync(indexName);

        public async Task<Index> CreateIndexAsync(Index index)
        {
            index = await _client.Indexes.CreateAsync(index).ConfigureAwait(false);
            _indexes.TryAdd(index.Name, index);
            return index;
        }

        public Task<Index> CreateIndexAsync(IndexDefinition indexDefinition)
            => CreateIndexAsync(indexDefinition.ToAzureIndex());

        public Task<Index> CreateIndexForLogAsync(string indexName)
            => CreateIndexAsync(FilterLogIndexUtil.Create(indexName));

        public Task<Index> CreateIndexForEventLogAsync<TEventDTO>(string indexName = null)
            where TEventDTO : class
            => CreateIndexAsync(new EventIndexDefinition<TEventDTO>(indexName));

        public Task<Index> CreateIndexForFunctionMessageAsync<TFunctionMessage>(string indexName = null)
            where TFunctionMessage : FunctionMessage
            => CreateIndexAsync(new FunctionIndexDefinition<TFunctionMessage>(indexName));

        public async Task<Index> GetIndexAsync(string indexName)
        {
            if (_indexes.TryGetValue(indexName, out var index))
            {
                return index;
            }

            index = await _client.Indexes.GetAsync(indexName).ConfigureAwait(false);

            if (index != null)
            {
                _indexes.TryAdd(indexName, index);
            }

            return index;
        }

        public async Task DeleteIndexAsync(string indexName)
        {
            if (await _client.Indexes.ExistsAsync(indexName).ConfigureAwait(false))
            {
                await _client.Indexes.DeleteAsync(indexName).ConfigureAwait(false);
            }

            _indexes.TryRemove(indexName, out _);
        }

        public IIndexer<TSource> CreateIndexer<TSource, TSearchDocument>(
            string indexName, Func<TSource, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TSource : class, new()
                where TSearchDocument : class
        {
            var indexClient = GetOrCreateIndexClient(indexName);
            var azureIndexer = new AzureIndexer<TSource, TSearchDocument>(indexClient, mapper, documentsPerBatch);
            _indexers.Add(azureIndexer);
            return azureIndexer;
        }

        public IIndexer<FilterLog> CreateIndexerForLog(string indexName, int documentsPerBatch = 1)
        {
            var indexClient = GetOrCreateIndexClient(indexName);
            var azureIndexer = new AzureFilterLogIndexer(indexClient, documentsPerBatch);
            _indexers.Add(azureIndexer);
            return azureIndexer;
        }

        public IIndexer<FilterLog> CreateIndexerForLog<TSearchDocument>(
            string indexName, Func<FilterLog, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TSearchDocument : class
        {
            var indexClient = GetOrCreateIndexClient(indexName);
            var azureIndexer = new AzureFilterLogIndexer<TSearchDocument>(indexClient, mapper, documentsPerBatch);
            _indexers.Add(azureIndexer);
            return azureIndexer;
        }

        public IIndexer<EventLog<TEventDTO>> CreateIndexerForEventLog<TEventDTO>(
            string indexName, int documentsPerBatch = 1)
                where TEventDTO : class, IEventDTO, new()
        {
            var indexClient = GetOrCreateIndexClient(indexName);
            var indexDefinition = new EventIndexDefinition<TEventDTO>(indexName);
            var azureIndexer = new AzureEventIndexer<TEventDTO>(indexClient, indexDefinition, documentsPerBatch);
            _indexers.Add(azureIndexer);
            return azureIndexer;
        }

        public IIndexer<EventLog<TEventDTO>> CreateIndexerForEventLog<TEventDTO, TSearchDocument>(
            string indexName, Func<EventLog<TEventDTO>, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TEventDTO : class, IEventDTO, new()
                where TSearchDocument : class
        {
            var indexClient = GetOrCreateIndexClient(indexName);
            var azureIndexer = new AzureEventIndexer<TEventDTO, TSearchDocument>(indexClient, mapper, documentsPerBatch);
            _indexers.Add(azureIndexer);
            return azureIndexer;
        }

        public IIndexer<TransactionReceiptVO> CreateIndexerForTransactionReceiptVO(
            string indexName, TransactionReceiptVOIndexDefinition indexDefinition, int documentsPerBatch = 1)
        {
            var indexClient = GetOrCreateIndexClient(indexName);
            var azureIndexer = new AzureTransactionReceiptVOIndexer(indexClient, indexDefinition, documentsPerBatch);
            _indexers.Add(azureIndexer);
            return azureIndexer;
        }

        public IIndexer<TransactionReceiptVO> CreateIndexerForTransactionReceiptVO<TSearchDocument>(
            string indexName, Func<TransactionReceiptVO, TSearchDocument> mapper, int documentsPerBatch = 1) where TSearchDocument : class
        {
            var indexClient = GetOrCreateIndexClient(indexName);
            var azureIndexer = new AzureTransactionReceiptVOIndexer<TSearchDocument>(indexClient, mapper, documentsPerBatch);
            _indexers.Add(azureIndexer);
            return azureIndexer;
        }

        public IIndexer<TransactionForFunctionVO<TFunctionMessage>> CreateIndexerForFunctionMessage<TFunctionMessage>(
            string indexName, int documentsPerBatch = 1)
                where TFunctionMessage : FunctionMessage, new()
        {
            var indexClient = GetOrCreateIndexClient(indexName);
            var indexDefinition = new FunctionIndexDefinition<TFunctionMessage>(indexName);
            var azureIndexer = new AzureFunctionIndexer<TFunctionMessage>(indexClient, indexDefinition, documentsPerBatch);
            _indexers.Add(azureIndexer);
            return azureIndexer;
        }

        public IIndexer<TransactionForFunctionVO<TFunctionMessage>> CreateIndexerForFunctionMessage<TFunctionMessage, TSearchDocument>(
            string indexName, Func<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TFunctionMessage : FunctionMessage, new()
                where TSearchDocument : class
        {
            var indexClient = GetOrCreateIndexClient(indexName);
            var azureIndexer = new AzureFunctionIndexer<TFunctionMessage, TSearchDocument>(indexClient, mapper, documentsPerBatch);
            _indexers.Add(azureIndexer);
            return azureIndexer;
        }

        public void Dispose()
        {
            foreach (var client in _clients.Values)
            {
                client.Dispose();
            }

            foreach (var indexer in _indexers)
            {
                indexer.Dispose();
            }

            ((IDisposable)_client)?.Dispose();
        }

        public IAzureIndexSearcher CreateIndexSearcher(Index index)
        {
            var client = GetOrCreateIndexClient(index.Name);
            return new AzureIndexSearcher(index, client);
        }
    }
}
