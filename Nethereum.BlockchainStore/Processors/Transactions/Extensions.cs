using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public static class Extensions
    {
        public static bool Ignore(this IEnumerable<ITransactionFilter> filters, Transaction transaction)
        {
            return filters.Any() && !filters.Any(f => f.IsMatch(transaction));
        }

        public static bool Ignore(this IEnumerable<ITransactionReceiptFilter> filters, TransactionReceipt transactionReceipt)
        {
            return filters.Any() && !filters.Any(f => f.IsMatch(transactionReceipt));
        }
    }
}
