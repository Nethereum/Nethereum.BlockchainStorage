using Nethereum.BlockchainStore.Handlers;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class TransactionLogFilter : Filter<TransactionLogWrapper>, ITransactionLogFilter
    {
        public TransactionLogFilter(Func<TransactionLogWrapper, Task<bool>> condition) : base(condition)
        {
        }

        public TransactionLogFilter(Func<TransactionLogWrapper, bool> condition) : base(condition)
        {
        }
    }
}
