using Nethereum.RPC.Eth.DTOs;
using System.Linq;

namespace Nethereum.BlockchainStore.Entities.Mapping
{
    public static class TransactionLogMapping
    {
        public static void Map(this TransactionLog transactionLog, Log log)
        {
            transactionLog.TransactionHash = log.TransactionHash;
            transactionLog.LogIndex = log.LogIndex.ToLong();
            transactionLog.Address = log.Address;
            transactionLog.Data = log.Data;

            transactionLog.EventHash = log.EventSignature;
            transactionLog.IndexVal1 = log.IndexedVal1;
            transactionLog.IndexVal2 = log.IndexedVal2;
            transactionLog.IndexVal3 = log.IndexedVal3;
        }
    }
}
