using System;
using System.Numerics;

namespace Nethereum.RPC.Eth.DTOs
{
    public static class Extensions
    {
        public const string EmptyAddressHex = "0x0";

        public static int TransactionCount(this Block block)
        {
            if (block is BlockWithTransactions b)
                return b.Transactions.Length;

            if (block is BlockWithTransactionHashes bh)
                return bh.TransactionHashes.Length;

            return 0;
        }

        public static bool IsAnEmptyAddress(this string address)
        {
            if(string.IsNullOrWhiteSpace(address))
                return true;

            return address == EmptyAddressHex;
        }

        public static bool IsNotAnEmptyAddress(this string address)
        {
            return !address.IsAnEmptyAddress();
        }

        public static string AddressValueOrEmpty(this string address)
        {
            return address.IsAnEmptyAddress() ? EmptyAddressHex : address;
        }

        public static bool EqualsAddress(this string address1, string address2)
        {
            return (address1.AddressValueOrEmpty())
                .Equals(address2.AddressValueOrEmpty(), StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsEmptyOrEqualsAddress(this string address1, string candidate)
        {
            return address1.IsAnEmptyAddress() || address1.EqualsAddress(candidate);
        }

        public static bool IsToAnEmptyAddress(this Transaction txn)
        {
            return txn.To.IsAnEmptyAddress();
        }

        public static bool IsToOrEmpty(this Transaction txn, string address)
        {
            return txn.To.IsEmptyOrEqualsAddress(address);
        }

        public static bool IsTo(this Transaction txn, string address)
        {
            return txn.To.EqualsAddress(address);
        }

        public static bool IsFrom(this Transaction txn, string address)
        {
            return txn.From.EqualsAddress(address);
        }

        public static bool IsFromAndTo(this Transaction txn, string from, string to)
        {
            return txn.IsFrom(from) && txn.IsTo(to);
        }

        public static bool IsContractAddressEmptyOrEqual(this TransactionReceipt receipt, string contractAddress)
        {
            return receipt.ContractAddress.IsEmptyOrEqualsAddress(contractAddress);
        }

        public static bool IsContractAddressEqual(this TransactionReceipt receipt, string address)
        {
            return receipt.ContractAddress.EqualsAddress(address);
        }

        public static bool IsForContractCreation(
            this Transaction transaction, TransactionReceipt transactionReceipt)
        {
            return transaction.To.IsAnEmptyAddress() && 
                   transactionReceipt.ContractAddress.IsNotAnEmptyAddress();
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
