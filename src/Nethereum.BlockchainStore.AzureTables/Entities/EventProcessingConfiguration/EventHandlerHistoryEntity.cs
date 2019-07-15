using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;

namespace Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration
{
    public class EventHandlerHistoryEntity : TableEntity, IEventHandlerHistoryDto
    {
        public long EventHandlerId
        {
            get => this.PartionKeyToLong();
            set => PartitionKey = value.ToString();
        }

        public string EventKey
        {
            get => this.RowKey;
            set => RowKey = value;
        }

        public long SubscriberId { get;set; }
        public long EventSubscriptionId { get; set; }
        public long Id { get; set; }
    }
}
