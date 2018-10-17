﻿using Nethereum.BlockchainProcessing.Processors;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Handlers
{
    public class TransactionLogWrapper
    {
        public TransactionLogWrapper()
        {}

        public TransactionLogWrapper(Transaction transaction, TransactionReceipt receipt, Log log)
        {
            Transaction = transaction;
            Receipt = receipt;
            Log = log;
        }

        public Transaction Transaction { get; private set; }
        public TransactionReceipt Receipt { get; private set; }
        public Log Log { get; private set; }
        public long LogIndex => Log == null ? -1 : (long)Log.LogIndex.Value;
        public string Address => Log?.Address;
        public string EventSignature => Log?.EventSignature;

        public virtual bool IsForEvent<TEvent>() where TEvent : new()
        {
            return Log?.IsLogForEvent<TEvent>() ?? false;
        }

        public virtual EventLog<TEvent> Decode<TEvent>() where TEvent : new()
        {
            return Log?.DecodeEvent<TEvent>();
        }

        public virtual bool IsTo(string toAddress)
        {
            return Transaction?.IsTo(toAddress) ?? false;
        }
    }
}