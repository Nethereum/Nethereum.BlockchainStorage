using System;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class TransactionFilter : Filter<Transaction>, ITransactionFilter
    {
        public TransactionFilter(Func<Transaction, bool> matchFunc) : base(matchFunc)
        {
        }
    }
}