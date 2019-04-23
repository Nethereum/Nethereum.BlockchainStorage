using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs;

namespace Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration
{
    public class SubscriberEntity : TableEntity, ISubscriberDto
    {
        public SubscriberEntity(){}
        public long PartitionId
        {
            get => this.PartionKeyToLong();
            set => PartitionKey = value.ToString();
        }

        public long Id
        {
            get => this.RowKeyToLong();
            set => RowKey = value.ToString();
        }

        public string Name { get; set; }

        public bool Disabled { get; set; }
    }
}
