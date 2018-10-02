using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class TransactionLogFilter : Filter<JObject>, ITransactionLogFilter
    {
        public TransactionLogFilter(Func<JObject, Task<bool>> condition) : base(condition)
        {
        }
    }
}
