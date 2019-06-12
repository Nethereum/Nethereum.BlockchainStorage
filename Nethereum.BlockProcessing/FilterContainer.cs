using System.Collections.Generic;
using Nethereum.BlockchainProcessing.Processors;
using Nethereum.BlockchainProcessing.Processors.Transactions;

namespace Nethereum.BlockchainProcessing.Processing
{
    public class FilterContainer
    {
        public IEnumerable<IBlockFilter> BlockFilters { get; }
        public IEnumerable<ITransactionFilter> TransactionFilters { get; }
        public IEnumerable<ITransactionReceiptFilter> TransactionReceiptFilters { get; }
        public IEnumerable<ITransactionAndReceiptFilter> TransactionAndReceiptFilters { get; }
        public IEnumerable<ITransactionLogFilter> TransactionLogFilters { get; }

        public FilterContainer(
            IEnumerable<IBlockFilter> blockFilters = null,
            IEnumerable<ITransactionFilter> transactionFilters = null,
            IEnumerable<ITransactionReceiptFilter> transactionReceiptFilters = null,
            IEnumerable<ITransactionAndReceiptFilter> transactionAndReceiptFilters = null,
            IEnumerable<ITransactionLogFilter> transactionLogFilters = null)
        {
            BlockFilters = blockFilters;
            TransactionFilters = transactionFilters;
            TransactionReceiptFilters = transactionReceiptFilters;
            TransactionAndReceiptFilters = transactionAndReceiptFilters;
            TransactionLogFilters = transactionLogFilters;
        }

        public FilterContainer(IBlockFilter blockFilter)
            : this(blockFilter, null)
        {
        }

        public FilterContainer(ITransactionFilter transactionFilter)
            : this(null, transactionFilter)
        {
        }

        public FilterContainer(ITransactionReceiptFilter transactionReceiptFilter)
            : this(null, null, transactionReceiptFilter)
        {
        }

        public FilterContainer(ITransactionAndReceiptFilter transactionAndReceiptFilter)
            : this(null, null, null, transactionAndReceiptFilter)
        {
        }

        public FilterContainer(ITransactionLogFilter transactionLogFilter)
            : this(null, null, null, null, transactionLogFilter)
        {
        }

        public FilterContainer(
            ITransactionFilter transactionFilter,
            ITransactionReceiptFilter transactionReceiptFilter)
            : this(null, transactionFilter, transactionReceiptFilter)
        {

        }

        public FilterContainer(
            ITransactionFilter transactionFilter,
            ITransactionReceiptFilter transactionReceiptFilter,
            ITransactionAndReceiptFilter transactionAndReceiptFilter)
            : this(null, transactionFilter, transactionReceiptFilter, transactionAndReceiptFilter)
        {

        }

        public FilterContainer(
            IBlockFilter blockFilter = null,
            ITransactionFilter transactionFilter = null,
            ITransactionReceiptFilter transactionReceiptFilter = null,
            ITransactionAndReceiptFilter transactionAndReceiptFilter = null,
            ITransactionLogFilter transactionLogFilter = null)
        {
            if (blockFilter != null)
            {
                BlockFilters = new List<IBlockFilter>(1) { blockFilter }; ;
            }

            if (transactionFilter != null)
            {
                TransactionFilters = new List<ITransactionFilter>(1)
                    {
                        transactionFilter
                    };
            }

            if (transactionReceiptFilter != null)
            {
                TransactionReceiptFilters = new List<ITransactionReceiptFilter>(1)
                    {
                        transactionReceiptFilter
                    };
            }

            if (transactionAndReceiptFilter != null)
            {
                TransactionAndReceiptFilters = new List<ITransactionAndReceiptFilter>(1)
                {
                    transactionAndReceiptFilter
                };
            }

            if (transactionLogFilter != null)
            {
                TransactionLogFilters = new List<ITransactionLogFilter>(1)
                    {
                        transactionLogFilter
                    };
            }
        }

    }
}
