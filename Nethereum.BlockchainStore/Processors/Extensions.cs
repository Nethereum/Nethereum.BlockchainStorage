using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Handlers;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.Processors
{
    public static class Extensions
    {
        public static async Task<bool> IgnoreAsync<T>(
            this IEnumerable<IFilter<T>> filters, T item)
        {
            var match = await filters.IsMatchAsync(item);
            return !match;
        }

        public static async Task<bool> IsMatchAsync<T>(
            this IEnumerable<IFilter<T>> filters, T item)
        {
            //match everything if we have no filters
            if (filters == null || !filters.Any()) return true;

            foreach (var filter in filters)
            {
                if(await filter.IsMatchAsync(item)) return true;
            }

            return false;
        }

        public static bool IsAnEmptyAddress(this string address)
        {
            if(string.IsNullOrWhiteSpace(address))
                return true;

            return address == "0x0";
        }

        public static bool IsNotAnEmptyAddress(this string address)
        {
            return !address.IsAnEmptyAddress();
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

        public static IEnumerable<TransactionLogWrapper> GetTransactionLogs(this Transaction transaction, TransactionReceipt receipt)
        {
            for (var i = 0; i < receipt.Logs?.Count; i++)
            {
                if (receipt.Logs[i] is JObject log)
                {
                    var typedLog = log.ToObject<Log>();

                    yield return
                            new TransactionLogWrapper(transaction, receipt, typedLog);
                }
            }
        }

        public static string[] GetAllRelatedAddresses(this Transaction tx, TransactionReceipt receipt)
        {
            if (tx == null)
                return Array.Empty<string>();

            var uniqueAddresses = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) 
                {tx.From};

            if (tx.To.IsNotAnEmptyAddress()) 
                uniqueAddresses.Add(tx.To);

            if (receipt != null)
            {
                if (receipt.ContractAddress.IsNotAnEmptyAddress())
                    uniqueAddresses.Add(receipt.ContractAddress);

                foreach (var log in tx.GetTransactionLogs(receipt))
                {
                    if (log.Address.IsNotAnEmptyAddress())
                        uniqueAddresses.Add(log.Address);
                }
            }

            return uniqueAddresses.ToArray();

        }
    }
}
