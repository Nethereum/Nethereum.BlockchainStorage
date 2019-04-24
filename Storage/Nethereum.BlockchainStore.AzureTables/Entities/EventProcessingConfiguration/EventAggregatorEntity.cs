using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using System;

namespace Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration
{
    public class EventAggregatorEntity : TableEntity, IEventAggregatorDto
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
            get
            {
                return Source.ToString();
            }
            set
            {
                Source = (AggregatorSource)Enum.Parse(typeof(AggregatorSource), value);
            }
        }

        public AggregatorSource Source {get;set; }

        public string OperationAsString
        {
            get
            {
                return Operation.ToString();
            }
            set
            {
                Operation = (AggregatorOperation)Enum.Parse(typeof(AggregatorOperation), value);
            }
        }

        public AggregatorOperation Operation { get; set; }

        public string DestinationAsString
        {
            get
            {
                return Destination.ToString();
            }
            set
            {
                Destination = (AggregatorDestination)Enum.Parse(typeof(AggregatorDestination), value);
            }
        }
        public AggregatorDestination Destination { get; set; }
        public int EventParameterNumber { get; set; }
        public string SourceKey { get; set; }
        public string OutputKey { get; set; }

    }
}
