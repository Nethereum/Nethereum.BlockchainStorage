using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Processors.Transactions
{
    public class TransactionFilter : Filter<Transaction>, ITransactionFilter
    {
        public TransactionFilter(){}

        public TransactionFilter(Func<Transaction, Task<bool>> condition) : base(condition){}

        public TransactionFilter(Func<Transaction, bool> condition) : base(condition){}

        public static TransactionFilter ValueGreaterThanZero()
        {
            return new TransactionFilter(tx => tx.Value.Value > 0);
        }

        public static TransactionFilter To(string toAddress)
        {
            return new TransactionFilter((t) => t.IsTo(toAddress));
        }

        public static TransactionFilter ToOrEmpty(string toAddress)
        {
            return new TransactionFilter((t) => t.IsToOrEmpty(toAddress));
        }

        public static TransactionFilter ToOrEmpty(IEnumerable<string> toAddresses)
        {
            return new TransactionFilter((t) => toAddresses.Any(t.IsToOrEmpty));
        }

        public static TransactionFilter To(IEnumerable<string> toAddresses)
        {
            return new TransactionFilter((t) => toAddresses.Any(t.IsTo));
        }

        public static TransactionFilter From(string fromAddress)
        {
            return new TransactionFilter((t) => t.IsFrom(fromAddress));
        }

        public static TransactionFilter From(IEnumerable<string> fromAddresses)
        {
            return new TransactionFilter((t) => fromAddresses.Any(t.IsFrom));
        }

        public static TransactionFilter FromAndTo(string fromAddress, string toAddress)
        {
            return new TransactionFilter((t) => t.IsFromAndTo(fromAddress, toAddress));
        }
    }
}