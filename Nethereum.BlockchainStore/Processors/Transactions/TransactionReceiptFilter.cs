using System;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class TransactionReceiptFilter : Filter<TransactionReceipt>, ITransactionReceiptFilter
    {
        public TransactionReceiptFilter(Func<TransactionReceipt, bool> matchFunc) : base(matchFunc)
        {
        }
    }
}