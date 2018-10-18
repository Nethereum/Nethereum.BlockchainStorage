using System;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Processors.Transactions
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

        public static TransactionAndReceiptFilter SentToOrCreatedContract(string contractAddress)
        {
            return new TransactionAndReceiptFilter(t =>
            {
                var (txn, receipt) = t;

                if (txn.IsToAnEmptyAddress())
                {
                    return receipt.IsContractAddressEqual(contractAddress);
                }

                return txn.IsTo(contractAddress);
            });
        }
    }
}
