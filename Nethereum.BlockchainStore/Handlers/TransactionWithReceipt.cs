using System;
using Nethereum.BlockchainStore.Processors;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;
using Nethereum.ABI.Model;
using Nethereum.Hex.HexConvertors.Extensions;

namespace Nethereum.BlockchainStore.Handlers
{
    public class TransactionWithReceipt
    {
        public TransactionWithReceipt()
        {
            
        }

        public TransactionWithReceipt(Transaction transaction, TransactionReceipt transactionReceipt, bool hasError, HexBigInteger blockTimestamp, string error = null, bool hasVmStack = false)
        {
            Transaction = transaction;
            TransactionReceipt = transactionReceipt;
            HasError = hasError;
            BlockTimestamp = blockTimestamp;
            Error = error;
            HasVmStack = hasVmStack;
        }

        public Transaction Transaction { get; private set; }
        public TransactionReceipt TransactionReceipt { get; private set; }
        public bool HasError { get; private set; }
        public HexBigInteger BlockTimestamp { get; private set; }
        public string Error { get; private set; }
        public bool HasVmStack { get; private set; }

        public string[] GetAllRelatedAddresses()
        {
            return Transaction?.GetAllRelatedAddresses(TransactionReceipt);
        }

        public Log[] Logs()
        {
            return Transaction?.GetTransactionLogs(TransactionReceipt).Select(l => l.Log).ToArray();
        }

        public List<EventLog<T>> GetEvents<T>(Event eventHandler) where T : new()
        {
            return eventHandler.DecodeAllEventsForEvent<T>(Logs());
        }

        public bool HasLogs()
        {
            return TransactionReceipt?.Logs?.Count > 0;
        }

        public FunctionABI GetFunction(Contract contract)
        {
            var inputFunctionSignature = Transaction.Input.Substring(0, 10);

            FunctionABI functionAbi = null;

            foreach (var func in contract.ContractBuilder.ContractABI.Functions)
            {
                var funcSig = func.Sha3Signature.EnsureHexPrefix();
                if (funcSig.Equals(inputFunctionSignature, StringComparison.InvariantCultureIgnoreCase))
                    functionAbi = func;
            }

            return functionAbi;
        }
    }

    public static class FunctionAbiExtensions{

        public static string NameAndParameters(this FunctionABI functionAbi)
        {
            return $"{functionAbi.Name}({string.Join(",", functionAbi.InputParameters.Select(p => p.ABIType.CanonicalName))})";
        }
    }
}
