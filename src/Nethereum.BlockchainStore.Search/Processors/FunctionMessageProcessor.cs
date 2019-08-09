using Nethereum.BlockchainProcessing.Processor;
using Nethereum.Contracts;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{
    public class FunctionMessageProcessor<TFunctionMessage> : TransactionReceiptVOProcessorHandler<TFunctionMessage> 
        where TFunctionMessage : FunctionMessage, new()
    {
        public FunctionMessageProcessor(
            IIndexer<TransactionForFunctionVO<TFunctionMessage>> indexer) :
                base((functionCall) => indexer.IndexAsync(functionCall))
        {
            Indexer = indexer;
        }

        public FunctionMessageProcessor(
            IIndexer<TransactionForFunctionVO<TFunctionMessage>> indexer,
            Func<TransactionForFunctionVO<TFunctionMessage>, Task<bool>> criteria) :
                base((functionCall) => indexer.IndexAsync(functionCall), criteria)
        {
            Indexer = indexer;
        }

        public FunctionMessageProcessor(
            IIndexer<TransactionForFunctionVO<TFunctionMessage>> indexer,
            Func<TransactionForFunctionVO<TFunctionMessage>, bool> criteria) :
                base((functionCall) => indexer.IndexAsync(functionCall), criteria)
        {
            Indexer = indexer;
        }

        public IIndexer<TransactionForFunctionVO<TFunctionMessage>> Indexer { get; }
    }
}
