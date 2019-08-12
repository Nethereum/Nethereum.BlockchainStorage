using Nethereum.BlockchainProcessing.Processor;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{
    public class TransactionReceiptSearchIndexProcessorHandler : ProcessorHandler<TransactionReceiptVO>
    {
        public TransactionReceiptSearchIndexProcessorHandler(
            IIndexer<TransactionReceiptVO> indexer) :
        base((eventLog) => indexer.IndexAsync(eventLog))
        {
            Indexer = indexer;
        }

        public TransactionReceiptSearchIndexProcessorHandler(
            IIndexer<TransactionReceiptVO> indexer,
            Func<TransactionReceiptVO, Task<bool>> asyncCriteria) :
                base((eventLog) => indexer.IndexAsync(eventLog), asyncCriteria)
        {
            Indexer = indexer;
        }

        public TransactionReceiptSearchIndexProcessorHandler(
            IIndexer<TransactionReceiptVO> eventIndexer,
            Func<TransactionReceiptVO, bool> criteria) :
                base((eventLog) => eventIndexer.IndexAsync(eventLog), criteria)
        {
            Indexer = eventIndexer;
        }

        public IIndexer<TransactionReceiptVO> Indexer { get; }
    }
}
