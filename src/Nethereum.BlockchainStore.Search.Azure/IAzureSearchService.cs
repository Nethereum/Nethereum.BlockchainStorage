using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processor;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureSearchService
    {
        Task<long> CountDocumentsAsync(string indexName);
        Task<Index> CreateIndexAsync(Index index);
        Task<Index> CreateIndexAsync(IndexDefinition indexDefinition);
        Task<Index> CreateIndexForEventAsync<TEventDTO>(string indexName = null) where TEventDTO : class;
        Task<Index> CreateIndexForFilterLogAsync(string indexName);

        ProcessorHandler<TSource> CreateProcessor<TSource, TSearchDocument>(
            Index index, Func<TSource, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TSource : class, new() where TSearchDocument : class;

        ProcessorHandler<TSource> CreateProcessor<TSource, TSearchDocument>(
            Index index, Func<TSource, bool> criteria, Func<TSource, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TSource : class, new() where TSearchDocument : class;

        ProcessorHandler<TSource> CreateProcessor<TSource, TSearchDocument>(
            Index index, Func<TSource, Task<bool>> criteria, Func<TSource, TSearchDocument> mapper, int documentsPerBatch = 1)
                where TSource : class, new() where TSearchDocument : class;

        FilterLogProcessor CreateLogProcessor(Index index, int documentsPerBatch = 1);
        FilterLogProcessor CreateLogProcessor(Index index, Func<FilterLog, bool> criteria, int documentsPerBatch = 1);
        FilterLogProcessor CreateLogProcessor(Index index, Func<FilterLog, Task<bool>> asyncCriteria, int documentsPerBatch = 1);
        EventLogProcessor<TEventDTO> CreateLogProcessor<TEventDTO, TSearchDocument>(Index index, Func<EventLog<TEventDTO>, TSearchDocument> mapper, int documentsPerBatch = 1)
            where TEventDTO : class, IEventDTO, new()
            where TSearchDocument : class;
        EventLogProcessor<TEventDTO> CreateLogProcessor<TEventDTO, TSearchDocument>(Index index, Func<EventLog<TEventDTO>, TSearchDocument> mapper, Func<EventLog<TEventDTO>, bool> criteria, int documentsPerBatch = 1)
            where TEventDTO : class, IEventDTO, new()
            where TSearchDocument : class;
        EventLogProcessor<TEventDTO> CreateLogProcessor<TEventDTO, TSearchDocument>(Index index, Func<EventLog<TEventDTO>, TSearchDocument> mapper, Func<EventLog<TEventDTO>, Task<bool>> asyncCriteria, int documentsPerBatch = 1)
            where TEventDTO : class, IEventDTO, new()
            where TSearchDocument : class;
        EventLogProcessor<TEventDTO> CreateLogProcessor<TEventDTO>(Index index, int documentsPerBatch = 1) where TEventDTO : class, IEventDTO, new();
        EventLogProcessor<TEventDTO> CreateLogProcessor<TEventDTO>(Index index, Func<EventLog<TEventDTO>, bool> criteria, int documentsPerBatch = 1) where TEventDTO : class, IEventDTO, new();
        EventLogProcessor<TEventDTO> CreateLogProcessor<TEventDTO>(Index index, Func<EventLog<TEventDTO>, Task<bool>> asyncCriteria, int documentsPerBatch = 1) where TEventDTO : class, IEventDTO, new();
        FilterLogProcessor CreateLogProcessor<TSearchDocument>(Index index, Func<FilterLog, TSearchDocument> mapper, int documentsPerBatch = 1) where TSearchDocument : class;
        FilterLogProcessor CreateLogProcessor<TSearchDocument>(Index index, Func<FilterLog, TSearchDocument> mapper, Func<FilterLog, bool> criteria, int documentsPerBatch = 1) where TSearchDocument : class;
        FilterLogProcessor CreateLogProcessor<TSearchDocument>(Index index, Func<FilterLog, TSearchDocument> mapper, Func<FilterLog, Task<bool>> asyncCriteria, int documentsPerBatch = 1) where TSearchDocument : class;

        TransactionReceiptVOProcessor CreateTransactionReceiptVOProcessor(Index index, TransactionReceiptVOIndexDefinition indexDefinition, int logsPerIndexBatch = 1);

        TransactionReceiptVOProcessor CreateTransactionReceiptVOProcessor(
            Index index, Func<TransactionReceiptVO, Task<bool>> asyncCriteria, TransactionReceiptVOIndexDefinition indexDefinition, int logsPerIndexBatch = 1);

        TransactionReceiptVOProcessor CreateTransactionReceiptVOProcessor(
            Index index, Func<TransactionReceiptVO, bool> criteria, TransactionReceiptVOIndexDefinition indexDefinition, int logsPerIndexBatch = 1);

        TransactionReceiptVOProcessor CreateTransactionReceiptVOProcessor<TSearchDocument>(
            Index index, Func<TransactionReceiptVO, Task<bool>> asyncCriteria, Func<TransactionReceiptVO, TSearchDocument> mapper, int logsPerIndexBatch = 1)
            where TSearchDocument : class;

        TransactionReceiptVOProcessor CreateTransactionReceiptVOProcessor<TSearchDocument>(
            Index index, Func<TransactionReceiptVO, bool> criteria, Func<TransactionReceiptVO, TSearchDocument> mapper, int logsPerIndexBatch = 1)
            where TSearchDocument : class;

        FunctionMessageProcessor<TFunctionMessage> CreateFunctionMessageProcessor<TFunctionMessage>(
            Index index, int logsPerIndexBatch = 1)
                where TFunctionMessage : FunctionMessage, new();

        FunctionMessageProcessor<TFunctionMessage> CreateFunctionMessageProcessor<TFunctionMessage>(
            Index index, Func<TransactionForFunctionVO<TFunctionMessage>, Task<bool>> asyncCriteria, int logsPerIndexBatch = 1)
                where TFunctionMessage : FunctionMessage, new();

        FunctionMessageProcessor<TFunctionMessage> CreateFunctionMessageProcessor<TFunctionMessage>(
            Index index, Func<TransactionForFunctionVO<TFunctionMessage>, bool> criteria, int logsPerIndexBatch = 1)
                where TFunctionMessage : FunctionMessage, new();

        FunctionMessageProcessor<TFunctionMessage> CreateFunctionMessageProcessor<TFunctionMessage, TSearchDocument>(
            Index index, Func<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument> mapper, int logsPerIndexBatch = 1)
                where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class;

        FunctionMessageProcessor<TFunctionMessage> CreateFunctionMessageProcessor<TFunctionMessage, TSearchDocument>(
            Index index, Func<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument> mapper, Func<TransactionForFunctionVO<TFunctionMessage>, Task<bool>> asyncCriteria, int logsPerIndexBatch = 1)
                where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class;

        FunctionMessageProcessor<TFunctionMessage> CreateFunctionMessageProcessor<TFunctionMessage, TSearchDocument>(
            Index index, Func<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument> mapper, Func<TransactionForFunctionVO<TFunctionMessage>, bool> criteria, int logsPerIndexBatch = 1)
                where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class;

        Task DeleteIndexAsync(string indexName);
        void Dispose();
        Task<Index> GetIndexAsync(string indexName);
        ISearchIndexClient GetOrCreateIndexClient(string indexName);
        Task<bool> IndexExistsAsync(string indexName);
    }
}