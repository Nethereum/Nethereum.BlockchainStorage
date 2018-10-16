using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Processors.Transactions
{
    public class TransactionReceiptFilter : Filter<TransactionReceipt>, ITransactionReceiptFilter
    {
        public TransactionReceiptFilter(Func<TransactionReceipt, Task<bool>> condition) : base(condition)
        {
        }

        public TransactionReceiptFilter(Func<TransactionReceipt, bool> condition) : base(condition)
        {
        }

        public static TransactionReceiptFilter ForContract(string contractAddress)
        {
            return new TransactionReceiptFilter(
                receipt => receipt.IsContractAddressEqual(contractAddress));
        }

        public static TransactionReceiptFilter ForContract(IEnumerable<string> contractAddresses)
        {
            return new TransactionReceiptFilter(
                receipt => contractAddresses.Any(receipt.IsContractAddressEqual));
        }

        public static TransactionReceiptFilter ForContractOrEmpty(IEnumerable<string> contractAddresses)
        {
            return new TransactionReceiptFilter(
                receipt => contractAddresses.Any(receipt.IsContractAddressEmptyOrEqual));
        }
    }
}