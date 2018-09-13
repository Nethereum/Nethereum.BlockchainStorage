using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.AzureTables.Entities
{
    public class TransactionLog : TableEntity
    {
        public TransactionLog(string hash, long logIndex)
        {
            TransactionHash = hash;
            LogIndex = logIndex;
        }

        public string TransactionHash
        {
            get => PartitionKey;
            set => PartitionKey = value.ToPartitionKey();
        }

        public long LogIndex
        {
            get => long.Parse(RowKey);
            set => RowKey = value.ToString().ToRowKey();
        } 

        public string Address { get; set; } = string.Empty;
        public string Topics { get; set; } = string.Empty;
        public string Topic0 { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;

        public static TransactionLog CreateTransactionLog(string transactionHash, long logIndex, JObject log)
        {
            var transactionLog = new TransactionLog(transactionHash, logIndex);
            transactionLog.InitLog(log);
            return transactionLog;
        }

        public void InitLog(JObject logObject)
        {
            Address = logObject["address"].Value<string>() ?? string.Empty;
            Data = logObject["data"].Value<string>() ?? string.Empty;

            if (logObject["topics"] is JArray topics)
            {
                Topics = topics.ToString();
                if (topics.Count > 0)
                    Topic0 = topics[0].ToString();
            }
        }
    }
}