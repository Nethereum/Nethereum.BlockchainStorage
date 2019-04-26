using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class EventHandlerRepository : OwnedRepository<IEventHandlerDto, EventHandlerEntity>, IEventHandlerRepository
    {
        public EventHandlerRepository(CloudTable table) : base(table)
        {
        }

        protected override EventHandlerEntity Map(IEventHandlerDto dto)
        {
            return new EventHandlerEntity
            {
                Id = dto.Id,
                EventSubscriptionId = dto.EventSubscriptionId,
                Disabled = dto.Disabled,
                HandlerType = dto.HandlerType,
                Order = dto.Order,
                SubscriberQueueId = dto.SubscriberQueueId,
                SubscriberRepositoryId = dto.SubscriberRepositoryId,
                SubscriberSearchIndexId = dto.SubscriberSearchIndexId
            };
        }
    }
}
