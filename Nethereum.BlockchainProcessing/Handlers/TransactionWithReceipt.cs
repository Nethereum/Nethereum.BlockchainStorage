using Nethereum.BlockchainProcessing.Processors;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Handlers
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

        public virtual string[] GetAllRelatedAddresses()
        {
            return Transaction?.GetAllRelatedAddresses(TransactionReceipt);
        }

        public virtual bool HasLogs()
        {
            return TransactionReceipt?.HasLogs() ?? false;
        }

        public virtual bool IsForFunction<TFunctionMessage>() where TFunctionMessage : FunctionMessage, new()
        {
            return Transaction?.IsTransactionForFunctionMessage<TFunctionMessage>() ?? false;
        }

        public virtual TFunctionMessage Decode<TFunctionMessage>() where TFunctionMessage : FunctionMessage, new()
        {
            return Transaction?.DecodeTransactionToFunctionMessage<TFunctionMessage>();
        }

    }
}
