using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;

namespace Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration
{
    public class SubscriberContractEntity : TableEntity, ISubscriberContractDto
    {
        public long Id
        {
            get => this.RowKeyToLong();
            set => RowKey = value.ToString();
        }

        public long SubscriberId
        {
            get => this.PartionKeyToLong();
            set => PartitionKey = value.ToString();
        }
        public string Abi { get;set;}
        public string Name { get; set; }

    }
}
