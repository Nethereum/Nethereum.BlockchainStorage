using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Newtonsoft.Json;
using System;

namespace Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration
{
    public class ContractQueryParameterEntity : TableEntity, IContractQueryParameterDto
    {
        public long ContractQueryId
        {
            get => this.PartionKeyToLong();
            set => PartitionKey = value.ToString();
        }

        public long Id
        {
            get => this.RowKeyToLong();
            set => RowKey = value.ToString();
        }

        public int Order {get;set; }

        public string SourceAsString
        {
            get
            {
                return Source.ToString();
            }
            set
            {
                Source = (EventValueSource)Enum.Parse(typeof(EventValueSource), value);
            }
        }

        public EventValueSource Source { get; set; }

        public string ValueAsJson
        {
            get { return Value == null ? null : JsonConvert.SerializeObject(Value);}
            set { Value = value == null ? null : JsonConvert.DeserializeObject(value);}
        }

        public object Value { get; set; }
        public int EventParameterNumber { get; set; }
        public string EventStateName { get; set; }

    }
}
