using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Handlers
{
    public class TransactionLogWrapper
    {
        public TransactionLogWrapper(Transaction transaction, TransactionReceipt receipt, Log log)
        {
            Transaction = transaction;
            Receipt = receipt;
            Log = log;
        }

        public Transaction Transaction { get; private set; }
        public TransactionReceipt Receipt { get; private set; }
        public Log Log { get; private set; }
        public long LogIndex => (long)Log.LogIndex.Value;
        public string Address => Log?.Address;
        public string EventSignature => Log?.EventSignature;

        public T DecodeEvent<T>(Event eventHandler) where T: new()
        {
            return Log.DecodeEvent<T>(eventHandler);
        }

    }
}