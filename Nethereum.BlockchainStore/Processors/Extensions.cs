using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nethereum.ABI.Model;
using Nethereum.BlockchainStore.Handlers;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;
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

        public static List<EventLog<T>> GetEvents<T>(this Transaction transaction, TransactionReceipt receipt, Event eventHandler) where T : new()
        {
            var logs = transaction.GetTransactionLogs(receipt).Select(l => l.Log).ToArray();
            return eventHandler.DecodeAllEventsForEvent<T>(logs);
        }

        public static string NameAndParameters(this FunctionABI functionAbi)
        {
            return $"{functionAbi.Name}({string.Join(",", functionAbi.InputParameters.Select(p => p.ABIType.CanonicalName))})";
        }

        public static FunctionABI FindFunction(this Transaction transaction, Contract contract)
        {
            return contract.FindFunctionBySignature(transaction.Input);
        }

        public static FunctionABI FindFunctionBySignature(this Contract contract, string sha3Signature)
        {
            if (sha3Signature == null || sha3Signature.Length < 10) return null;

            var inputFunctionSignature = sha3Signature.Substring(0, 10);
            FunctionABI functionAbi = null;

            foreach (var func in contract.ContractBuilder.ContractABI.Functions)
            {
                var funcSig = func.Sha3Signature.EnsureHexPrefix();
                if (funcSig.Equals(inputFunctionSignature, StringComparison.InvariantCultureIgnoreCase))
                    functionAbi = func;
            }

            return functionAbi;
        }

        public static bool HasLogs(this TransactionReceipt receipt)
        {
            return receipt.Logs?.Count > 0;
        }

    }
}
