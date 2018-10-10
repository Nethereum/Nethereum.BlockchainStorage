using Nethereum.BlockchainStore.Handlers;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class TransactionLogFilter : Filter<TransactionLog>, ITransactionLogFilter
    {
        public TransactionLogFilter(Func<TransactionLog, Task<bool>> condition) : base(condition)
        {
        }

        public TransactionLogFilter(Func<TransactionLog, bool> condition) : base(condition)
        {
        }
    }
}
