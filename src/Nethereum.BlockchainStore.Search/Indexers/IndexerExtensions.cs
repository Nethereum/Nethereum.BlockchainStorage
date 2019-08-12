using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processor;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{
    public static class IndexerExtensions
    {
        public static ProcessorHandler<TSource> CreateProcessorHandler<TSource>(
            this IIndexer<TSource> indexer)
                where TSource : class, new()
        {
            return new ProcessorHandler<TSource>((x) => indexer.IndexAsync(x));
        }

        public static ProcessorHandler<TSource> CreateProcessorHandler<TSource>(
            this IIndexer<TSource> indexer,
            Func<TSource, bool> criteria)
                where TSource : class, new()
        {
            return new ProcessorHandler<TSource>((x) => indexer.IndexAsync(x), criteria);
        }

        public static ProcessorHandler<TSource> CreateProcessorHandler<TSource>(
            this IIndexer<TSource> indexer,
            Func<TSource, Task<bool>> criteria)
                where TSource : class, new()
        {
            return new ProcessorHandler<TSource>((x) => indexer.IndexAsync(x), criteria);
        }

        public static ProcessorHandler<FilterLog> CreateProcessorHandler(
            this IIndexer<FilterLog> indexer)
        {
            return new FilterLogSearchIndexProcessorHandler(indexer);
        }

        public static ProcessorHandler<FilterLog> CreateProcessorHandler(
            this IIndexer<FilterLog> indexer, Func<FilterLog, bool> criteria)
        {
            return new FilterLogSearchIndexProcessorHandler(indexer, criteria);
        }

        public static ProcessorHandler<FilterLog> CreateProcessorHandler(
            this IIndexer<FilterLog> indexer, Func<FilterLog, Task<bool>> asyncCriteria)
        {
            return new FilterLogSearchIndexProcessorHandler(indexer, asyncCriteria);
        }

        public static ProcessorHandler<FilterLog> CreateProcessorHandler<TEventDTO>(
            this IIndexer<EventLog<TEventDTO>> indexer)
                where TEventDTO : class, IEventDTO, new()
        {
            return new EventLogSearchIndexProcessorHandler<TEventDTO>(indexer);
        }

        public static ProcessorHandler<FilterLog> CreateProcessorHandler<TEventDTO>(
            this IIndexer<EventLog<TEventDTO>> indexer,
            Func<EventLog<TEventDTO>, bool> criteria)
                where TEventDTO : class, IEventDTO, new()
        {
            return new EventLogSearchIndexProcessorHandler<TEventDTO>(indexer, criteria);
        }

        public static ProcessorHandler<FilterLog> CreateProcessorHandler<TEventDTO>(
            this IIndexer<EventLog<TEventDTO>> indexer,
            Func<EventLog<TEventDTO>, Task<bool>> asyncCriteria)
                where TEventDTO : class, IEventDTO, new()
        {
            return new EventLogSearchIndexProcessorHandler<TEventDTO>(indexer, asyncCriteria);
        }

        public static ProcessorHandler<TransactionReceiptVO> CreateProcessorHandler(
            this IIndexer<TransactionReceiptVO> indexer)
        {
            return new TransactionReceiptSearchIndexProcessorHandler(indexer);
        }

        public static ProcessorHandler<TransactionReceiptVO> CreateProcessorHandler(
            this IIndexer<TransactionReceiptVO> indexer,
            Func<TransactionReceiptVO, bool> criteria)
        {
            return new TransactionReceiptSearchIndexProcessorHandler(indexer, criteria);
        }

        public static ProcessorHandler<TransactionReceiptVO> CreateProcessorHandler(
            this IIndexer<TransactionReceiptVO> indexer,
            Func<TransactionReceiptVO, Task<bool>> asyncCriteria)
        {
            return new TransactionReceiptSearchIndexProcessorHandler(indexer, asyncCriteria);
        }

        public static ProcessorHandler<TransactionReceiptVO> CreateProcessorHandler<TFunctionMessage>(
            this IIndexer<TransactionForFunctionVO<TFunctionMessage>> indexer)
                where TFunctionMessage : FunctionMessage, new()
        {
            return new FunctionMessageSearchIndexProcessorHandler<TFunctionMessage>(indexer);
        }

        public static ProcessorHandler<TransactionReceiptVO> CreateProcessorHandler<TFunctionMessage>(
            this IIndexer<TransactionForFunctionVO<TFunctionMessage>> indexer,
            Func<TransactionForFunctionVO<TFunctionMessage>, bool> criteria)
                where TFunctionMessage : FunctionMessage, new()
        {
            return new FunctionMessageSearchIndexProcessorHandler<TFunctionMessage>(indexer, criteria);
        }

        public static ProcessorHandler<TransactionReceiptVO> CreateProcessorHandler<TFunctionMessage>(
            this IIndexer<TransactionForFunctionVO<TFunctionMessage>> indexer,
            Func<TransactionForFunctionVO<TFunctionMessage>, Task<bool>> asyncCriteria)
                where TFunctionMessage : FunctionMessage, new()
        {
            return new FunctionMessageSearchIndexProcessorHandler<TFunctionMessage>(indexer, asyncCriteria);
        }
    }
}
