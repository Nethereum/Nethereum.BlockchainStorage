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

        public List<EventLog<T>> GetEvents<T>(Event eventHandler) where T : new()
        {
            return Transaction?.GetEvents<T>(TransactionReceipt, eventHandler);
        }

        public bool HasLogs()
        {
            return TransactionReceipt?.HasLogs() ?? false;
        }

        public FunctionABI FindFunction(Contract contract)
        {
            return Transaction?.FindFunction(contract);
        }

        public string GetFunctionCaption(Contract contract)
        {
            return FindFunction(contract)?.NameAndParameters();
        }
    }
}
