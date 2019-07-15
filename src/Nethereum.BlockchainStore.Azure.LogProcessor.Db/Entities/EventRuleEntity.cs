using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using System;

namespace Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration
{
    public class EventRuleEntity : TableEntity, IEventRuleDto
    {
        public long Id
        {
            get => this.RowKeyToLong();
            set => RowKey = value.ToString();
        }
        public long EventHandlerId
        {
            get => this.PartionKeyToLong();
            set => PartitionKey = value.ToString();
        }

        public string SourceAsString
        {
            get {  return Source.ToString();}
            set {  Source = (EventRuleSource)Enum.Parse(typeof(EventRuleSource), value);}
        }

        public EventRuleSource Source { get; set; }

        public string TypeAsString
        {
            get { return Type.ToString(); }
            set { Type = (EventRuleType)Enum.Parse(typeof(EventRuleType), value); }
        }
        public EventRuleType Type { get; set; }
        public string InputName { get; set; }
        public int EventParameterNumber { get; set; }
        public string Value {get;set; }
    }
}
