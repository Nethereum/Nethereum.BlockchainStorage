using System;
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

        public static TransactionAndReceiptFilter CreatedOrCalledContract(string contractAddress)
        {
            return new TransactionAndReceiptFilter(t =>
            {
                if (t.Item1.To.IsAnEmptyAddress())
                {
                    return t.Item2.ContractAddress.IsNotAnEmptyAddress() &&
                           t.Item2.ContractAddress.Equals(contractAddress, StringComparison.InvariantCultureIgnoreCase);
                }

                return contractAddress.Equals(t.Item1.To, StringComparison.InvariantCultureIgnoreCase);
            });
        }
    }
}
