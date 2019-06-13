using System.Numerics;

namespace Nethereum.RPC.Eth.DTOs
{
    public static class TransactionReceiptExtensions
    {
        public static bool IsContractAddressEmptyOrEqual(this TransactionReceipt receipt, string contractAddress)
        {
            return receipt.ContractAddress.IsEmptyOrEqualsAddress(contractAddress);
        }

        public static bool IsContractAddressEqual(this TransactionReceipt receipt, string address)
        {
            return receipt.ContractAddress.EqualsAddress(address);
        }

        public static bool Succeeded(this TransactionReceipt receipt)
        {
            return receipt.Status.Value == BigInteger.One;
        }

        public static bool Failed(this TransactionReceipt receipt)
        {
            return !receipt.Succeeded();
        }

        public static bool HasLogs(this TransactionReceipt receipt)
        {
            return receipt.Logs?.Count > 0;
        }
    }

}
