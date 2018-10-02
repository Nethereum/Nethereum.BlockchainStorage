using System;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class TransactionReceiptFilter : Filter<TransactionReceipt>, ITransactionReceiptFilter
    {
        public TransactionReceiptFilter(Func<TransactionReceipt, Task<bool>> condition) : base(condition)
        {
        }

        public static TransactionReceiptFilter OnlyNewContracts()
        {
            return new TransactionReceiptFilter(
                receipt => Task.FromResult(!string.IsNullOrEmpty(receipt.ContractAddress)));
        }
    }
}