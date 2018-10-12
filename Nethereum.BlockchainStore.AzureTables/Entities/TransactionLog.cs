using System;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.Entities;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.AzureTables.Entities
{
    public class TransactionLog : TableEntity, ITransactionLogView
    {
        private string _topics = string.Empty;
        private string _eventHash = string.Empty;
        private string _indexVal1 = string.Empty;
        private string _indexVal2 = string.Empty;
        private string _indexVal3 = string.Empty;

        public TransactionLog()
        {
        }

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

        public string Topics
        {
            get => _topics;
            set
            { 
                _topics = value;  
                SetValuesBasedOnTopics();
            }
        }

        private void SetValuesBasedOnTopics()
        {
            _eventHash = string.Empty;
            _indexVal1 = string.Empty;
            _indexVal2 = string.Empty;
            _indexVal3 = string.Empty;

            var topics = string.IsNullOrEmpty(Topics) ? (JArray)null : JArray.Parse(Topics);

            if (topics?.Count > 0)
            {
                _eventHash = topics[0].Value<string>();

                if (topics.Count > 1)
                    _indexVal1 = topics[1].Value<string>();

                if (topics.Count > 2)
                    _indexVal2 = topics[2].Value<string>();

                if (topics.Count > 3)
                    _indexVal3 = topics[3].Value<string>();
            }            
        }

        public string Topic0 { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;

        public string EventHash => _eventHash;
        public string IndexVal1 => _indexVal1;
        public string IndexVal2 => _indexVal2;
        public string IndexVal3 => _indexVal3;

        public static TransactionLog CreateTransactionLog(Log log)
        {
            var transactionLog = new TransactionLog(log.TransactionHash, (long)log.LogIndex.Value);
            transactionLog.InitLog(log);
            return transactionLog;
        }

        public void InitLog(Log log)
        {
            Address = log?.Address;
            Data = log?.Data;
            Topic0 = log?.EventSignature;

            if(log?.Topics != null)
                Topics = JArray.FromObject(log.Topics).ToString();
        }
    }
}