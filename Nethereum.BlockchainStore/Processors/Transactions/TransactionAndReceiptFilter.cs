using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class TransactionAndReceiptFilter : 
        Filter<(Transaction, TransactionReceipt)>, ITransactionAndReceiptFilter
    {
        public TransactionAndReceiptFilter(System.Func<(Transaction, TransactionReceipt), Task<bool>> condition) : base(condition)
        {
        }

        public TransactionAndReceiptFilter(System.Func<(Transaction, TransactionReceipt), bool> condition) : base(condition)
        {
        }
    }
}
