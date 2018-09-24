using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.Entities.Mapping
{
    public static class TransactionLogMapping
    {
        public static void Map(this TransactionLog transactionLog, string transactionHash, long logIndex, JObject log)
        {
            transactionLog.TransactionHash = transactionHash;
            transactionLog.LogIndex = logIndex;
            transactionLog.Address = log["address"].Value<string>() ?? string.Empty;
            transactionLog.Data = log["data"].Value<string>() ?? string.Empty;
            var topics = log["topics"] as JArray;

            if (topics?.Count > 0)
            {
                transactionLog.EventHash = topics[0].Value<string>();

                if (topics.Count > 1)
                    transactionLog.IndexVal1 = topics[1].Value<string>();

                if (topics.Count > 2)
                    transactionLog.IndexVal2 = topics[2].Value<string>();

                if (topics.Count > 3)
                    transactionLog.IndexVal3 = topics[3].Value<string>();
            }
        }
    }
}
