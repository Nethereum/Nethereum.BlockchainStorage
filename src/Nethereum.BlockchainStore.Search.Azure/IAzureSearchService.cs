using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processor;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureSearchService: IDisposable
    {
        Task<long> CountDocumentsAsync(string indexName);
        Task<Index> CreateIndexAsync(Index index);
        Task<Index> CreateIndexAsync(IndexDefinition indexDefinition);
        Task<Index> CreateIndexForEventAsync<TEventDTO>(string indexName = null) where TEventDTO : class;
        Task<Index> CreateIndexForFilterLogAsync(string indexName);
        Task<Index> CreateIndexForFunctionAsync<TFunctionMessage>(string indexName = null)
            where TFunctionMessage : FunctionMessage;
        Task DeleteIndexAsync(string indexName);
        Task<Index> GetIndexAsync(string indexName);
        ISearchIndexClient GetOrCreateIndexClient(string indexName);
        Task<bool> IndexExistsAsync(string indexName);
        ProcessorHandler<TSource> CreateProcessor<TSource, TSearchDocument>(
            Index index, Func<TSource, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TSource : class, new() where TSearchDocument : class;

        ProcessorHandler<TSource> CreateProcessor<TSource, TSearchDocument>(
            Index index, Func<TSource, bool> criteria, Func<TSource, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TSource : class, new() where TSearchDocument : class;

        ProcessorHandler<TSource> CreateProcessor<TSource, TSearchDocument>(
            Index index, Func<TSource, Task<bool>> criteria, Func<TSource, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TSource : class, new() where TSearchDocument : class;

        FilterLogSearchIndexProcessor CreateLogProcessor(Index index, int documentsPerBatch = 1);
        FilterLogSearchIndexProcessor CreateLogProcessor(Index index, Func<FilterLog, bool> criteria, int documentsPerBatch = 1);
        FilterLogSearchIndexProcessor CreateLogProcessor(Index index, Func<FilterLog, Task<bool>> asyncCriteria, int documentsPerBatch = 1);
        EventLogSearchIndexProcessor<TEventDTO> CreateLogProcessor<TEventDTO, TSearchDocument>(Index index, Func<EventLog<TEventDTO>, TSearchDocument> mapper, int documentsPerBatch = 1)
            where TEventDTO : class, IEventDTO, new()
            where TSearchDocument : class;
        EventLogSearchIndexProcessor<TEventDTO> CreateLogProcessor<TEventDTO, TSearchDocument>(Index index, Func<EventLog<TEventDTO>, TSearchDocument> mapper, Func<EventLog<TEventDTO>, bool> criteria, int documentsPerBatch = 1)
            where TEventDTO : class, IEventDTO, new()
            where TSearchDocument : class;
        EventLogSearchIndexProcessor<TEventDTO> CreateLogProcessor<TEventDTO, TSearchDocument>(Index index, Func<EventLog<TEventDTO>, TSearchDocument> mapper, Func<EventLog<TEventDTO>, Task<bool>> asyncCriteria, int documentsPerBatch = 1)
            where TEventDTO : class, IEventDTO, new()
            where TSearchDocument : class;
        EventLogSearchIndexProcessor<TEventDTO> CreateLogProcessor<TEventDTO>(Index index, int documentsPerBatch = 1) where TEventDTO : class, IEventDTO, new();
        EventLogSearchIndexProcessor<TEventDTO> CreateLogProcessor<TEventDTO>(Index index, Func<EventLog<TEventDTO>, bool> criteria, int documentsPerBatch = 1) where TEventDTO : class, IEventDTO, new();
        EventLogSearchIndexProcessor<TEventDTO> CreateLogProcessor<TEventDTO>(Index index, Func<EventLog<TEventDTO>, Task<bool>> asyncCriteria, int documentsPerBatch = 1) where TEventDTO : class, IEventDTO, new();
        FilterLogSearchIndexProcessor CreateLogProcessor<TSearchDocument>(Index index, Func<FilterLog, TSearchDocument> mapper, int documentsPerBatch = 1) where TSearchDocument : class;
        FilterLogSearchIndexProcessor CreateLogProcessor<TSearchDocument>(Index index, Func<FilterLog, TSearchDocument> mapper, Func<FilterLog, bool> criteria, int documentsPerBatch = 1) where TSearchDocument : class;
        FilterLogSearchIndexProcessor CreateLogProcessor<TSearchDocument>(Index index, Func<FilterLog, TSearchDocument> mapper, Func<FilterLog, Task<bool>> asyncCriteria, int documentsPerBatch = 1) where TSearchDocument : class;

        TransactionReceiptSearchIndexProcessor CreateTransactionReceiptVOProcessor(Index index, TransactionReceiptVOIndexDefinition indexDefinition, int logsPerIndexBatch = 1);

        TransactionReceiptSearchIndexProcessor CreateTransactionReceiptVOProcessor(
            Index index, Func<TransactionReceiptVO, Task<bool>> asyncCriteria, TransactionReceiptVOIndexDefinition indexDefinition, int logsPerIndexBatch = 1);

        TransactionReceiptSearchIndexProcessor CreateTransactionReceiptVOProcessor(
            Index index, Func<TransactionReceiptVO, bool> criteria, TransactionReceiptVOIndexDefinition indexDefinition, int logsPerIndexBatch = 1);

        TransactionReceiptSearchIndexProcessor CreateTransactionReceiptVOProcessor<TSearchDocument>(
            Index index, Func<TransactionReceiptVO, Task<bool>> asyncCriteria, Func<TransactionReceiptVO, TSearchDocument> mapper, int logsPerIndexBatch = 1)
            where TSearchDocument : class;

        TransactionReceiptSearchIndexProcessor CreateTransactionReceiptVOProcessor<TSearchDocument>(
            Index index, Func<TransactionReceiptVO, bool> criteria, Func<TransactionReceiptVO, TSearchDocument> mapper, int logsPerIndexBatch = 1)
            where TSearchDocument : class;

        FunctionMessageSearchIndexProcessor<TFunctionMessage> CreateFunctionMessageProcessor<TFunctionMessage>(
            Index index, int logsPerIndexBatch = 1)
                where TFunctionMessage : FunctionMessage, new();

        FunctionMessageSearchIndexProcessor<TFunctionMessage> CreateFunctionMessageProcessor<TFunctionMessage>(
            Index index, Func<TransactionForFunctionVO<TFunctionMessage>, Task<bool>> asyncCriteria, int logsPerIndexBatch = 1)
                where TFunctionMessage : FunctionMessage, new();

        FunctionMessageSearchIndexProcessor<TFunctionMessage> CreateFunctionMessageProcessor<TFunctionMessage>(
            Index index, Func<TransactionForFunctionVO<TFunctionMessage>, bool> criteria, int logsPerIndexBatch = 1)
                where TFunctionMessage : FunctionMessage, new();

        FunctionMessageSearchIndexProcessor<TFunctionMessage> CreateFunctionMessageProcessor<TFunctionMessage, TSearchDocument>(
            Index index, Func<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument> mapper, int logsPerIndexBatch = 1)
                where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class;

        FunctionMessageSearchIndexProcessor<TFunctionMessage> CreateFunctionMessageProcessor<TFunctionMessage, TSearchDocument>(
            Index index, Func<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument> mapper, Func<TransactionForFunctionVO<TFunctionMessage>, Task<bool>> asyncCriteria, int logsPerIndexBatch = 1)
                where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class;

        FunctionMessageSearchIndexProcessor<TFunctionMessage> CreateFunctionMessageProcessor<TFunctionMessage, TSearchDocument>(
            Index index, Func<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument> mapper, Func<TransactionForFunctionVO<TFunctionMessage>, bool> criteria, int logsPerIndexBatch = 1)
                where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class;

        AzureIndexer<TEventDTO, TSearchDocument> CreateIndexer<TEventDTO, TSearchDocument>(
                   Index index, Func<TEventDTO, TSearchDocument> mapper, int documentsPerBatch)
                       where TEventDTO : class, new()
                       where TSearchDocument : class;

        AzureFilterLogIndexer CreateFilterLogIndexer(Index index, int documentsPerBatch);

        AzureFilterLogIndexer<TSearchDocument> CreateFilterLogIndexer<TSearchDocument>(
            Index index, Func<FilterLog, TSearchDocument> mapper, int documentsPerBatch)
                where TSearchDocument : class;

        AzureEventIndexer<TEventDTO> CreateEventIndexer<TEventDTO>(
            Index index, int documentsPerBatch)
                where TEventDTO : class, IEventDTO, new();

        AzureEventIndexer<TEventDTO, TSearchDocument> CreateEventIndexer<TEventDTO, TSearchDocument>(
            Index index, Func<EventLog<TEventDTO>, TSearchDocument> mapper, int documentsPerBatch)
                where TEventDTO : class, IEventDTO, new()
                where TSearchDocument : class;

        AzureTransactionReceiptVOIndexer CreateTransactionReceiptVOIndexer(
            Index index, TransactionReceiptVOIndexDefinition indexDefinition, int documentsPerBatch);

        AzureTransactionReceiptVOIndexer<TSearchDocument> CreateTransactionReceiptVOIndexer<TSearchDocument>(
            Index index, Func<TransactionReceiptVO, TSearchDocument> mapper, int documentsPerBatch) where TSearchDocument : class;

        AzureFunctionIndexer<TFunctionMessage> CreateFunctionMessageIndexer<TFunctionMessage>(
            Index index, int documentsPerBatch)
                where TFunctionMessage : FunctionMessage, new();

        AzureFunctionIndexer<TFunctionMessage, TSearchDocument> CreateFunctionMessageIndexer<TFunctionMessage, TSearchDocument>(
            Index index, Func<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument> mapper, int documentsPerBatch)
                where TFunctionMessage : FunctionMessage, new()
                where TSearchDocument : class;
    }
}