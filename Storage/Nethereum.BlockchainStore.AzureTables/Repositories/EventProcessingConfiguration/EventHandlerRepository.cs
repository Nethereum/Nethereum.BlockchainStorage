using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class EventHandlerRepository : AzureTableRepository<EventHandlerEntity>, IEventHandlerRepository
    {
        public EventHandlerRepository(CloudTable table) : base(table)
        {
        }

        public static EventHandlerEntity Map(IEventHandlerDto dto)
        {
            if (dto is EventHandlerEntity entity) return entity;

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

        public async Task<IEventHandlerDto[]> GetEventHandlersAsync(long eventSubscriptionId)
        {
            return await GetManyAsync(eventSubscriptionId.ToString());
        }

        public async Task<IEventHandlerDto> UpsertAsync(IEventHandlerDto dto)
        {
            var entity = Map(dto);
            return await base.UpsertAsync(entity) as IEventHandlerDto;
        }
    }
}
