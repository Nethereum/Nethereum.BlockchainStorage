using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Processors.Transactions;
using System.Collections.Generic;

namespace Nethereum.BlockchainStore.Processing
{
    public class FilterContainer
    {
        public FilterContainer(IBlockFilter blockFilter):this(blockFilter, null)
        {
        }

        public FilterContainer(ITransactionFilter transactionFilter):this(null, transactionFilter)
        {
        }

        public FilterContainer(ITransactionReceiptFilter transactionReceiptFilter):this(null, null, transactionReceiptFilter)
        {
        }

        public FilterContainer(ITransactionLogFilter transactionLogFilter):this(null, null, null, transactionLogFilter)
        {
        }

        public FilterContainer(ITransactionFilter transactionFilter, ITransactionReceiptFilter transactionReceiptFilter):this(null, transactionFilter, transactionReceiptFilter)
        {
                
        }

        public FilterContainer(IBlockFilter blockFilter, ITransactionFilter transactionFilter, ITransactionReceiptFilter transactionReceiptFilter = null, ITransactionLogFilter transactionLogFilter = null)
        {
            if (blockFilter != null)
            {
                var blockFilters = new List<IBlockFilter>(1);
                blockFilters.Add(blockFilter);
                BlockFilters = blockFilters;
            }

            if (transactionFilter != null)
            {
                var transactionFilters = new List<ITransactionFilter>(1);
                transactionFilters.Add(transactionFilter);
                TransactionFilters = transactionFilters;
            }

            if (transactionReceiptFilter != null)
            {
                var transactionReceiptFilters = new List<ITransactionReceiptFilter>(1);
                transactionReceiptFilters.Add(transactionReceiptFilter);
                TransactionReceiptFilters = transactionReceiptFilters;
            }

            if (transactionLogFilter != null)
            {
                var transactionLogFilters = new List<ITransactionLogFilter>(1);
                transactionLogFilters.Add(transactionLogFilter);
                TransactionLogFilters = transactionLogFilters;
            }
        }

        public FilterContainer(            
            IEnumerable<IBlockFilter> blockFilters = null,
            IEnumerable<ITransactionFilter> transactionFilters = null,
            IEnumerable<ITransactionReceiptFilter> transactionReceiptFilters = null,
            IEnumerable<ITransactionLogFilter> transactionLogFilters = null)
        {
            BlockFilters = blockFilters;
            TransactionFilters = transactionFilters;
            TransactionReceiptFilters = transactionReceiptFilters;
            TransactionLogFilters = transactionLogFilters;
        }

        public IEnumerable<IBlockFilter> BlockFilters { get; }
        public IEnumerable<ITransactionFilter> TransactionFilters { get; }
        public IEnumerable<ITransactionReceiptFilter> TransactionReceiptFilters { get; }
        public IEnumerable<ITransactionLogFilter> TransactionLogFilters { get; }
    }
}
