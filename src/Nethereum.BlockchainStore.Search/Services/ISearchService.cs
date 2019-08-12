using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.Services
{
    public interface ISearchService: IDisposable
    {
        Task<long> CountDocumentsAsync(string indexName);
        Task DeleteIndexAsync(string indexName);

        Task<bool> IndexExistsAsync(string indexName);

        IIndexer<TSource> CreateIndexer<TSource, TSearchDocument>(
                   string indexName, Func<TSource, TSearchDocument> mapper, int documentsPerBatch = 1)
                       where TSource : class, new()
                       where TSearchDocument : class;

        IIndexer<FilterLog> CreateIndexerForLog(
            string indexName, int documentsPerBatch);

        IIndexer<FilterLog> CreateIndexerForLog<TSearchDocument>(
            string indexName, Func<FilterLog, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TSearchDocument : class;

        IIndexer<EventLog<TEventDTO>> CreateIndexerForEventLog<TEventDTO>(
            string indexName, int documentsPerBatch)
                where TEventDTO : class, IEventDTO, new();

        IIndexer<EventLog<TEventDTO>> CreateIndexerForEventLog<TEventDTO, TSearchDocument>(
            string indexName, Func<EventLog<TEventDTO>, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TEventDTO : class, IEventDTO, new()
                where TSearchDocument : class;

        IIndexer<TransactionReceiptVO> CreateIndexerForTransactionReceiptVO(
            string indexName, TransactionReceiptVOIndexDefinition indexDefinition, int documentsPerBatch = 1);

        IIndexer<TransactionReceiptVO> CreateIndexerForTransactionReceiptVO<TSearchDocument>(
            string indexName, Func<TransactionReceiptVO, TSearchDocument> mapper, int documentsPerBatch = 1) 
                where TSearchDocument : class;

        IIndexer<TransactionForFunctionVO<TFunctionMessage>> CreateIndexerForFunctionMessage<TFunctionMessage>(
            string indexName, int documentsPerBatch = 1)
                where TFunctionMessage : FunctionMessage, new();

        IIndexer<TransactionForFunctionVO<TFunctionMessage>> CreateIndexerForFunctionMessage<TFunctionMessage, TSearchDocument>(
            string indexName, Func<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TFunctionMessage : FunctionMessage, new()
                where TSearchDocument : class;

    }
}