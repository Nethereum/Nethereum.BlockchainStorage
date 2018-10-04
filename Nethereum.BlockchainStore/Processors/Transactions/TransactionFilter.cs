using System;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class TransactionFilter : Filter<Transaction>, ITransactionFilter
    {
        public TransactionFilter(Func<Transaction, Task<bool>> condition) : base(condition)
        {
        }

        public TransactionFilter(Func<Transaction, bool> condition) : base(condition)
        {
        }

        public static TransactionFilter ValueGreaterThanZero()
        {
            return new TransactionFilter(tx => Task.FromResult(tx.Value.Value > 0));
        }

        public static TransactionFilter To(string toAddress)
        {
            return new TransactionFilter((t) => t.To?.Equals(toAddress, StringComparison.InvariantCultureIgnoreCase) ?? false);
        }
    }
}