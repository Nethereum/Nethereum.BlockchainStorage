using System;
using Nethereum.BlockchainStore.Processors;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

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
            return Transaction.GetAllRelatedAddresses(TransactionReceipt);
        }
    }
}
