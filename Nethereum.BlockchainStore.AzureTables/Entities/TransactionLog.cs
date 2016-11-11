using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;
using Wintellect;
using Wintellect.Azure.Storage.Table;

namespace Nethereum.BlockchainStore.Entities
{
    public class TransactionLog : TableEntityBase
    {
        public TransactionLog(AzureTable azureTable, DynamicTableEntity dynamicTableEntity = null)
            : base(azureTable, dynamicTableEntity)
        {
        }

        public string TransactionHash
        {
            get { return Get(string.Empty); }
            set
            {
                PartitionKey = value.ToLowerInvariant().HtmlEncode();
                Set(value);
            }
        }

        public long LogIndex
        {
            get { return Get(0); }
            set
            {
                RowKey = value.ToString();
                Set(value);
            }
        }

        public string Address
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string Topics
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string Topic0
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public string Data
        {
            get { return Get(string.Empty); }
            set { Set(value); }
        }

        public static TransactionLog CreateTransactionLog(AzureTable logTable, string transactionHash, long logIndex,
            JObject log)
        {
            var transactionLog = new TransactionLog(logTable) {TransactionHash = transactionHash, LogIndex = logIndex};
            transactionLog.InitLog(log);
            return transactionLog;
        }

        public void InitLog(JObject logObject)
        {
            Address = logObject["address"].Value<string>() ?? string.Empty;
            Data = logObject["data"].Value<string>() ?? string.Empty;
            var topics = logObject["topics"] as JArray;
            if (topics != null)
            {
                Topics = topics.ToString();
                if (topics.Count > 0)
                    Topic0 = topics[0].ToString();
            }
        }
    }
}