using System;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class TransactionFilter : Filter<Transaction>, ITransactionFilter
    {
        public TransactionFilter(Func<Transaction, Task<bool>> matchFunc) : base(matchFunc)
        {
        }

        public static TransactionFilter ValueGreaterThanZero()
        {
            return new TransactionFilter(tx => Task.FromResult(tx.Value.Value > 0));
        }
    }
}