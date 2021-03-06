﻿using Nest;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainStore.Search.Elastic;
using Nethereum.BlockchainStore.Search.Services;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.ElasticSearch
{
    public class ElasticSearchService : IElasticSearchService
    {
        private readonly IElasticClient _elasticClient;
        private readonly List<IDisposable> _indexers;

        public ElasticSearchService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
            _indexers = new List<IDisposable>();
        }

        public async Task<long> CountDocumentsAsync(string indexName)
        {
            var countResponse = await _elasticClient.CountAsync<object>(new CountRequest(indexName));
            return countResponse.Count;
        }

        public async Task CreateIfNotExists(IndexDefinition searchIndexDefinition)
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

        public async Task CreateIfNotExists(string indexName)
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

        public Task DeleteIndexAsync(IndexDefinition searchIndex) => DeleteIndexAsync(searchIndex.ElasticIndexName());

        public async Task DeleteIndexAsync(string indexName)
        {
            var existsResponse = await _elasticClient.IndexExistsAsync(new IndexExistsRequest(indexName));
            if (existsResponse.Exists)
            {
                await _elasticClient.DeleteIndexAsync(indexName);
            }
        }

        public void Dispose()
        {
            foreach(var indexer in _indexers)
            {
                indexer.Dispose();
            }    
        }

        public async Task<bool> IndexExistsAsync(string indexName) 
        {
            var existsResponse = await _elasticClient.IndexExistsAsync(new IndexExistsRequest(indexName.ToElasticName())).ConfigureAwait(false);
            return existsResponse.Exists;
        }

        public IIndexer<TSource> CreateIndexer<TSource, TSearchDocument>(
            string indexName, Func<TSource, TSearchDocument> mapper, int documentsPerBatch)
                where TSource : class where TSearchDocument : class, IHasId
        {
            var indexer = new ElasticIndexer<TSource, TSearchDocument>(indexName, _elasticClient, mapper, documentsPerBatch);
            _indexers.Add(indexer);
            return indexer;
        }

        public IIndexer<FilterLog> CreateIndexerForLog(string indexName, int documentsPerBatch)
        {
            var indexer = new ElasticFilterLogIndexer(indexName, _elasticClient, documentsPerBatch);
            _indexers.Add(indexer);
            return indexer;
        }

        public IIndexer<FilterLog> CreateIndexerForLog<TSearchDocument>(
            string indexName, Func<FilterLog, TSearchDocument> mapper, int documentsPerBatch)
                where TSearchDocument : class, IHasId
        {
            var indexer = new ElasticFilterLogIndexer<TSearchDocument>(indexName, _elasticClient, mapper, documentsPerBatch);
            _indexers.Add(indexer);
            return indexer;
        }

        public IIndexer<EventLog<TEventDTO>> CreateIndexerForEventLog<TEventDTO>(string indexName, int documentsPerBatch)
            where TEventDTO : class, IEventDTO, new()
        {
            var indexDefinition = new EventIndexDefinition<TEventDTO>(indexName);
            var indexer = new ElasticEventIndexer<TEventDTO>(indexName, _elasticClient, indexDefinition, documentsPerBatch);
            _indexers.Add(indexer);
            return indexer;
        }

        public IIndexer<EventLog<TEventDTO>> CreateIndexerForEventLog<TEventDTO, TSearchDocument>(
            string indexName, Func<EventLog<TEventDTO>, TSearchDocument> mapper, int documentsPerBatch)
                where TEventDTO : class, IEventDTO, new() where TSearchDocument:  class, IHasId
        {
            var indexer = new ElasticEventIndexer<TEventDTO, TSearchDocument>(indexName, _elasticClient, mapper, documentsPerBatch);
            _indexers.Add(indexer);
            return indexer;
        }

        public IIndexer<TransactionReceiptVO> CreateIndexerForTransactionReceiptVO(
            string indexName, TransactionReceiptVOIndexDefinition indexDefinition, int documentsPerBatch)
        {
            var indexer = new ElasticTransactionReceiptVOIndexer(indexName, _elasticClient, indexDefinition, documentsPerBatch);
            _indexers.Add(indexer);
            return indexer;
        }

        public IIndexer<TransactionReceiptVO> CreateIndexerForTransactionReceiptVO<TSearchDocument>(
            string indexName, Func<TransactionReceiptVO, TSearchDocument> mapper, int documentsPerBatch)
                where TSearchDocument: class, IHasId
        {
            var indexer = new ElasticTransactionReceiptVOIndexer<TSearchDocument>(indexName, _elasticClient, mapper, documentsPerBatch);
            _indexers.Add(indexer);
            return indexer;
        }

        public IIndexer<TransactionForFunctionVO<TFunctionMessage>> CreateIndexerForFunctionMessage<TFunctionMessage>(
            string indexName, int documentsPerBatch)
             where TFunctionMessage : FunctionMessage, new()
        {
            var indexDefinition = new FunctionIndexDefinition<TFunctionMessage>(indexName);
            var indexer = new ElasticFunctionIndexer<TFunctionMessage>(indexName, _elasticClient, indexDefinition, documentsPerBatch);
            _indexers.Add(indexer);
            return indexer;
        }

        public IIndexer<TransactionForFunctionVO<TFunctionMessage>> CreateIndexerForFunctionMessage<TFunctionMessage, TSearchDocument>(
            string indexName, Func<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument> mapper, int documentsPerBatch)
                where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class, IHasId
        {
            var indexer = new ElasticFunctionIndexer<TFunctionMessage, TSearchDocument>(indexName, _elasticClient, mapper, documentsPerBatch);
            _indexers.Add(indexer);
            return indexer;
        }
    }
}
