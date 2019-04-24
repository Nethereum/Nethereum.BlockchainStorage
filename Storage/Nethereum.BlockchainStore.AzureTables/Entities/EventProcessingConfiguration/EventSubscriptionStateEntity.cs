using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration
{
    public class EventSubscriptionStateEntity : TableEntity, IEventSubscriptionStateDto
    {
        public long Id
        {
            get => this.RowKeyToLong();
            set => RowKey = value.ToString();
        }

        public long EventSubscriptionId
        {
            get => this.PartionKeyToLong();
            set => PartitionKey = value.ToString();
        }

        public string ValuesAsJson
        {
            get {  return JsonConvert.SerializeObject(Values);}
            set {  Values = JsonConvert.DeserializeObject<Dictionary<string, object>>(value); }
        }

        public Dictionary<string, object> Values {get;set; } = new Dictionary<string, object>();

    }
}
