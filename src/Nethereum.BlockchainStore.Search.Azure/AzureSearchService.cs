using Microsoft.Azure.Search;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processor;
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
            using (var client = _client.Indexes.GetClient(indexName))
            {
                return await client.Documents.CountAsync().ConfigureAwait(false);
            }
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

        public Task<Index> CreateIndexForFilterLogAsync(string indexName)
            => CreateIndexAsync(FilterLogIndexUtil.Create(indexName));

        public Task<Index> CreateIndexForEventAsync<TEventDTO>(string indexName = null)
            where TEventDTO : class
            => CreateIndexAsync(new EventIndexDefinition<TEventDTO>(indexName));

        public Task<Index> CreateIndexForFunctionAsync<TFunctionMessage>(string indexName = null)
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

        public ProcessorHandler<TSource> CreateProcessor<TSource, TSearchDocument>(
            Index index, Func<TSource, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TSource : class, new() where TSearchDocument : class
        {
            var azureIndexer = CreateIndexer(index, mapper, documentsPerBatch);
            return new ProcessorHandler<TSource>((x) => azureIndexer.IndexAsync(x));
        }

        public ProcessorHandler<TSource> CreateProcessor<TSource, TSearchDocument>(
            Index index, Func<TSource, bool> criteria, Func<TSource, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TSource : class, new() where TSearchDocument : class
        {
            var azureIndexer = CreateIndexer(index, mapper, documentsPerBatch);
            return new ProcessorHandler<TSource>((x) => azureIndexer.IndexAsync(x), criteria);
        }

        public ProcessorHandler<TSource> CreateProcessor<TSource, TSearchDocument>(
            Index index, Func<TSource, Task<bool>> asyncCriteria, Func<TSource, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TSource : class, new() where TSearchDocument : class
        {
            var azureIndexer = CreateIndexer(index, mapper, documentsPerBatch);
            return new ProcessorHandler<TSource>((x) => azureIndexer.IndexAsync(x), asyncCriteria);
        }

        public FilterLogSearchIndexProcessor CreateLogProcessor(Index index, int documentsPerBatch = 1)
        {
            var azureIndexer = CreateFilterLogIndexer(index, documentsPerBatch);
            return new FilterLogSearchIndexProcessor(azureIndexer);
        }

        public FilterLogSearchIndexProcessor CreateLogProcessor(
            Index index, Func<FilterLog, Task<bool>> asyncCriteria, int documentsPerBatch = 1)
        {
            var azureIndexer = CreateFilterLogIndexer(index, documentsPerBatch);
            return new FilterLogSearchIndexProcessor(azureIndexer, asyncCriteria);
        }

        public FilterLogSearchIndexProcessor CreateLogProcessor(
            Index index, Func<FilterLog, bool> criteria, int documentsPerBatch = 1)
        {
            var azureIndexer = CreateFilterLogIndexer(index, documentsPerBatch);
            return new FilterLogSearchIndexProcessor(azureIndexer, criteria);
        }

        public FilterLogSearchIndexProcessor CreateLogProcessor<TSearchDocument>(
            Index index, Func<FilterLog, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TSearchDocument : class
        {
            var azureIndexer = CreateFilterLogIndexer(index, mapper, documentsPerBatch);
            return new FilterLogSearchIndexProcessor(azureIndexer);
        }

        public FilterLogSearchIndexProcessor CreateLogProcessor<TSearchDocument>(
            Index index, Func<FilterLog, TSearchDocument> mapper, Func<FilterLog, Task<bool>> asyncCriteria, int documentsPerBatch = 1)
                where TSearchDocument : class
        {
            var azureIndexer = CreateFilterLogIndexer(index, mapper, documentsPerBatch);
            return new FilterLogSearchIndexProcessor(azureIndexer, asyncCriteria);
        }

        public FilterLogSearchIndexProcessor CreateLogProcessor<TSearchDocument>(
            Index index, Func<FilterLog, TSearchDocument> mapper, Func<FilterLog, bool> criteria, int documentsPerBatch = 1)
                where TSearchDocument : class
        {
            var azureIndexer = CreateFilterLogIndexer(index, mapper, documentsPerBatch);
            return new FilterLogSearchIndexProcessor(azureIndexer, criteria);
        }

        public EventLogSearchIndexProcessor<TEventDTO> CreateLogProcessor<TEventDTO>(
            Index index, int documentsPerBatch = 1)
                where TEventDTO : class, IEventDTO, new()
        {
            var azureIndexer = CreateEventIndexer<TEventDTO>(index, documentsPerBatch);
            return new EventLogSearchIndexProcessor<TEventDTO>(azureIndexer);
        }

        public EventLogSearchIndexProcessor<TEventDTO> CreateLogProcessor<TEventDTO>(
            Index index, Func<EventLog<TEventDTO>, Task<bool>> asyncCriteria, int documentsPerBatch = 1)
                where TEventDTO : class, IEventDTO, new()
        {
            var azureIndexer = CreateEventIndexer<TEventDTO>(index, documentsPerBatch);
            return new EventLogSearchIndexProcessor<TEventDTO>(azureIndexer, asyncCriteria);
        }

        public EventLogSearchIndexProcessor<TEventDTO> CreateLogProcessor<TEventDTO>(
            Index index, Func<EventLog<TEventDTO>, bool> criteria, int documentsPerBatch = 1)
                where TEventDTO : class, IEventDTO, new()
        {
            var azureIndexer = CreateEventIndexer<TEventDTO>(index, documentsPerBatch);
            return new EventLogSearchIndexProcessor<TEventDTO>(azureIndexer, criteria);
        }

        public EventLogSearchIndexProcessor<TEventDTO> CreateLogProcessor<TEventDTO, TSearchDocument>(
            Index index, Func<EventLog<TEventDTO>, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TEventDTO : class, IEventDTO, new() where TSearchDocument : class
        {
            var azureIndexer = CreateEventIndexer(index, mapper, documentsPerBatch);
            return new EventLogSearchIndexProcessor<TEventDTO>(azureIndexer);
        }

        public EventLogSearchIndexProcessor<TEventDTO> CreateLogProcessor<TEventDTO, TSearchDocument>(
            Index index, Func<EventLog<TEventDTO>, TSearchDocument> mapper, Func<EventLog<TEventDTO>, Task<bool>> asyncCriteria, int documentsPerBatch = 1)
                where TEventDTO : class, IEventDTO, new() where TSearchDocument : class
        {
            var azureIndexer = CreateEventIndexer(index, mapper, documentsPerBatch);
            return new EventLogSearchIndexProcessor<TEventDTO>(azureIndexer, asyncCriteria);
        }

        public EventLogSearchIndexProcessor<TEventDTO> CreateLogProcessor<TEventDTO, TSearchDocument>(
            Index index, Func<EventLog<TEventDTO>, TSearchDocument> mapper, Func<EventLog<TEventDTO>, bool> criteria, int documentsPerBatch = 1)
                where TEventDTO : class, IEventDTO, new() where TSearchDocument : class
        {
            var azureIndexer = CreateEventIndexer(index, mapper, documentsPerBatch);
            return new EventLogSearchIndexProcessor<TEventDTO>(azureIndexer, criteria);
        }

        public TransactionReceiptSearchIndexProcessor CreateTransactionReceiptVOProcessor(Index index, TransactionReceiptVOIndexDefinition indexDefinition, int documentsPerBatch = 1)
        {
            var azureIndexer = CreateTransactionReceiptVOIndexer(index, indexDefinition, documentsPerBatch);
            return new TransactionReceiptSearchIndexProcessor(azureIndexer);
        }

        public TransactionReceiptSearchIndexProcessor CreateTransactionReceiptVOProcessor(
            Index index, Func<TransactionReceiptVO, Task<bool>> asyncCriteria, TransactionReceiptVOIndexDefinition indexDefinition, int documentsPerBatch = 1)
        {
            var azureIndexer = CreateTransactionReceiptVOIndexer(index, indexDefinition, documentsPerBatch);
            return new TransactionReceiptSearchIndexProcessor(azureIndexer, asyncCriteria);
        }

        public TransactionReceiptSearchIndexProcessor CreateTransactionReceiptVOProcessor(
            Index index, Func<TransactionReceiptVO, bool> criteria, TransactionReceiptVOIndexDefinition indexDefinition, int documentsPerBatch = 1)
        {
            var azureIndexer = CreateTransactionReceiptVOIndexer(index, indexDefinition, documentsPerBatch);
            return new TransactionReceiptSearchIndexProcessor(azureIndexer, criteria);
        }

        public TransactionReceiptSearchIndexProcessor CreateTransactionReceiptVOProcessor<TSearchDocument>(
            Index index, Func<TransactionReceiptVO, Task<bool>> asyncCriteria, Func<TransactionReceiptVO, TSearchDocument> mapper, int documentsPerBatch = 1) 
            where TSearchDocument : class
        {
            var azureIndexer = CreateTransactionReceiptVOIndexer(index, mapper, documentsPerBatch);
            return new TransactionReceiptSearchIndexProcessor(azureIndexer, asyncCriteria);
        }

        public TransactionReceiptSearchIndexProcessor CreateTransactionReceiptVOProcessor<TSearchDocument>(
            Index index, Func<TransactionReceiptVO, bool> criteria, Func<TransactionReceiptVO, TSearchDocument> mapper, int documentsPerBatch = 1)
            where TSearchDocument : class
        {
            var azureIndexer = CreateTransactionReceiptVOIndexer(index, mapper, documentsPerBatch);
            return new TransactionReceiptSearchIndexProcessor(azureIndexer, criteria);
        }

        public FunctionMessageSearchIndexProcessor<TFunctionMessage> CreateFunctionMessageProcessor<TFunctionMessage>(
            Index index, int documentsPerBatch = 1)
                where TFunctionMessage : FunctionMessage, new()
        {
            var azureIndexer = CreateFunctionMessageIndexer<TFunctionMessage>(index, documentsPerBatch);
            return new FunctionMessageSearchIndexProcessor<TFunctionMessage>(azureIndexer);
        }

        public FunctionMessageSearchIndexProcessor<TFunctionMessage> CreateFunctionMessageProcessor<TFunctionMessage>(
            Index index, Func<TransactionForFunctionVO<TFunctionMessage>, Task<bool>> asyncCriteria, int documentsPerBatch = 1)
                where TFunctionMessage : FunctionMessage, new()
        {
            var azureIndexer = CreateFunctionMessageIndexer<TFunctionMessage>(index, documentsPerBatch);
            return new FunctionMessageSearchIndexProcessor<TFunctionMessage>(azureIndexer, asyncCriteria);
        }

        public FunctionMessageSearchIndexProcessor<TFunctionMessage> CreateFunctionMessageProcessor<TFunctionMessage>(
            Index index, Func<TransactionForFunctionVO<TFunctionMessage>, bool> criteria, int documentsPerBatch = 1)
                where TFunctionMessage : FunctionMessage, new()
        {
            var azureIndexer = CreateFunctionMessageIndexer<TFunctionMessage>(index, documentsPerBatch);
            return new FunctionMessageSearchIndexProcessor<TFunctionMessage>(azureIndexer, criteria);
        }

        public FunctionMessageSearchIndexProcessor<TFunctionMessage> CreateFunctionMessageProcessor<TFunctionMessage, TSearchDocument>(
            Index index, Func<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class
        {
            var azureIndexer = CreateFunctionMessageIndexer(index, mapper, documentsPerBatch);
            return new FunctionMessageSearchIndexProcessor<TFunctionMessage>(azureIndexer);
        }

        public FunctionMessageSearchIndexProcessor<TFunctionMessage> CreateFunctionMessageProcessor<TFunctionMessage, TSearchDocument>(
            Index index, Func<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument> mapper, Func<TransactionForFunctionVO<TFunctionMessage>, Task<bool>> asyncCriteria, int documentsPerBatch = 1)
                where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class
        {
            var azureIndexer = CreateFunctionMessageIndexer(index, mapper, documentsPerBatch);
            return new FunctionMessageSearchIndexProcessor<TFunctionMessage>(azureIndexer, asyncCriteria);
        }

        public FunctionMessageSearchIndexProcessor<TFunctionMessage> CreateFunctionMessageProcessor<TFunctionMessage, TSearchDocument>(
            Index index, Func<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument> mapper, Func<TransactionForFunctionVO<TFunctionMessage>, bool> criteria, int documentsPerBatch = 1)
                where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class
        {
            var azureIndexer = CreateFunctionMessageIndexer(index, mapper, documentsPerBatch);
            return new FunctionMessageSearchIndexProcessor<TFunctionMessage>(azureIndexer, criteria);
        }

        public AzureIndexer<TEventDTO, TSearchDocument> CreateIndexer<TEventDTO, TSearchDocument>(
            Index index, Func<TEventDTO, TSearchDocument> mapper, int documentsPerBatch)
                where TEventDTO : class, new()
                where TSearchDocument : class
        {
            var indexClient = GetOrCreateIndexClient(index.Name);
            var azureIndexer = new AzureIndexer<TEventDTO, TSearchDocument>(index, indexClient, mapper, documentsPerBatch);
            _indexers.Add(azureIndexer);
            return azureIndexer;
        }

        public AzureFilterLogIndexer CreateFilterLogIndexer(Index index, int documentsPerBatch)
        {
            var indexClient = GetOrCreateIndexClient(index.Name);
            var azureIndexer = new AzureFilterLogIndexer(index, indexClient, documentsPerBatch);
            _indexers.Add(azureIndexer);
            return azureIndexer;
        }

        public AzureFilterLogIndexer<TSearchDocument> CreateFilterLogIndexer<TSearchDocument>(
            Index index, Func<FilterLog, TSearchDocument> mapper, int documentsPerBatch)
                where TSearchDocument : class
        {
            var indexClient = GetOrCreateIndexClient(index.Name);
            var azureIndexer = new AzureFilterLogIndexer<TSearchDocument>(index, indexClient, mapper, documentsPerBatch);
            _indexers.Add(azureIndexer);
            return azureIndexer;
        }

        public AzureEventIndexer<TEventDTO> CreateEventIndexer<TEventDTO>(
            Index index, int documentsPerBatch)
                where TEventDTO : class, IEventDTO, new()
        {
            var indexClient = GetOrCreateIndexClient(index.Name);
            var indexDefinition = new EventIndexDefinition<TEventDTO>(index.Name);
            var azureIndexer = new AzureEventIndexer<TEventDTO>(index, indexClient, indexDefinition, documentsPerBatch);
            _indexers.Add(azureIndexer);
            return azureIndexer;
        }

        public AzureEventIndexer<TEventDTO, TSearchDocument> CreateEventIndexer<TEventDTO, TSearchDocument>(
            Index index, Func<EventLog<TEventDTO>, TSearchDocument> mapper, int documentsPerBatch)
                where TEventDTO : class, IEventDTO, new()
                where TSearchDocument : class
        {
            var indexClient = GetOrCreateIndexClient(index.Name);
            var azureIndexer = new AzureEventIndexer<TEventDTO, TSearchDocument>(index, indexClient, mapper, documentsPerBatch);
            _indexers.Add(azureIndexer);
            return azureIndexer;
        }

        public AzureTransactionReceiptVOIndexer CreateTransactionReceiptVOIndexer(
            Index index, TransactionReceiptVOIndexDefinition indexDefinition, int documentsPerBatch)
        {
            var indexClient = GetOrCreateIndexClient(index.Name);
            var azureIndexer = new AzureTransactionReceiptVOIndexer(index, indexClient, indexDefinition, documentsPerBatch);
            _indexers.Add(azureIndexer);
            return azureIndexer;
        }

        public AzureTransactionReceiptVOIndexer<TSearchDocument> CreateTransactionReceiptVOIndexer<TSearchDocument>(
            Index index, Func<TransactionReceiptVO, TSearchDocument> mapper, int documentsPerBatch) where TSearchDocument : class
        {
            var indexClient = GetOrCreateIndexClient(index.Name);
            var azureIndexer = new AzureTransactionReceiptVOIndexer<TSearchDocument>(index, indexClient, mapper, documentsPerBatch);
            _indexers.Add(azureIndexer);
            return azureIndexer;
        }

        public AzureFunctionIndexer<TFunctionMessage> CreateFunctionMessageIndexer<TFunctionMessage>(
            Index index, int documentsPerBatch)
                where TFunctionMessage : FunctionMessage, new()
        {
            var indexClient = GetOrCreateIndexClient(index.Name);
            var indexDefinition = new FunctionIndexDefinition<TFunctionMessage>(index.Name);
            var azureIndexer = new AzureFunctionIndexer<TFunctionMessage>(index, indexClient, indexDefinition, documentsPerBatch);
            _indexers.Add(azureIndexer);
            return azureIndexer;
        }

        public AzureFunctionIndexer<TFunctionMessage, TSearchDocument> CreateFunctionMessageIndexer<TFunctionMessage, TSearchDocument>(
            Index index, Func<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument> mapper, int documentsPerBatch)
                where TFunctionMessage : FunctionMessage, new()
                where TSearchDocument : class
        {
            var indexClient = GetOrCreateIndexClient(index.Name);
            var azureIndexer = new AzureFunctionIndexer<TFunctionMessage, TSearchDocument>(index, indexClient, mapper, documentsPerBatch);
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

    }
}
