using System;
using System.Numerics;
using Nethereum.BlockchainProcessing.Processors;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Handlers
{
    public class TransactionWithReceipt
    {
        static readonly HexBigInteger UndefinedBlockNumber = new HexBigInteger(BigInteger.Zero);
        
        public TransactionWithReceipt()
        {
            
        }

        public TransactionWithReceipt(
            Transaction transaction, 
            TransactionReceipt transactionReceipt, 
            bool hasError, 
            HexBigInteger blockTimestamp, 
            string error = null, 
            bool hasVmStack = false)
        {
            Transaction = transaction;
            TransactionReceipt = transactionReceipt;
            HasError = hasError;
            BlockTimestamp = blockTimestamp;
            Error = error;
            HasVmStack = hasVmStack;
        }

        public Transaction Transaction { get; protected set; }
        public TransactionReceipt TransactionReceipt { get; protected set; }
        public bool HasError { get; protected set; }
        public HexBigInteger BlockTimestamp { get; protected set; }
        public string Error { get; protected set; }
        public bool HasVmStack { get; protected set; }

        public virtual HexBigInteger BlockNumber => Transaction?.BlockNumber ?? UndefinedBlockNumber;
        public virtual string TransactionHash => Transaction?.TransactionHash;
        public virtual bool Succeeded => TransactionReceipt?.Succeeded() ?? false;
        public virtual bool Failed => !Succeeded;

        public virtual string[] GetAllRelatedAddresses()
        {
            return Transaction?.GetAllRelatedAddresses(TransactionReceipt) ?? Array.Empty<string>();
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
