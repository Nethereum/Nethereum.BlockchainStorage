using Nethereum.BlockchainProcessing.Processor;
using Nethereum.Contracts;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{
    public class FunctionMessageSearchIndexProcessorHandler<TFunctionMessage> : TransactionReceiptVOProcessorHandler<TFunctionMessage> 
        where TFunctionMessage : FunctionMessage, new()
    {
        public FunctionMessageSearchIndexProcessorHandler(
            IIndexer<TransactionForFunctionVO<TFunctionMessage>> indexer) :
                base((functionCall) => indexer.IndexAsync(functionCall))
        {
            Indexer = indexer;
        }

        public FunctionMessageSearchIndexProcessorHandler(
            IIndexer<TransactionForFunctionVO<TFunctionMessage>> indexer,
            Func<TransactionForFunctionVO<TFunctionMessage>, Task<bool>> criteria) :
                base((functionCall) => indexer.IndexAsync(functionCall), criteria)
        {
            Indexer = indexer;
        }

        public FunctionMessageSearchIndexProcessorHandler(
            IIndexer<TransactionForFunctionVO<TFunctionMessage>> indexer,
            Func<TransactionForFunctionVO<TFunctionMessage>, bool> criteria) :
                base((functionCall) => indexer.IndexAsync(functionCall), criteria)
        {
            Indexer = indexer;
        }

        public IIndexer<TransactionForFunctionVO<TFunctionMessage>> Indexer { get; }
    }
}
