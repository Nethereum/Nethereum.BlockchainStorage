using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class EventSubscriptionStateRepository : AzureTableRepository<EventSubscriptionStateEntity>, IEventSubscriptionStateRepository
    {
        public EventSubscriptionStateRepository(CloudTable table) : base(table)
        {
        }

        public async Task<IEventSubscriptionStateDto> GetOrCreateEventSubscriptionStateAsync(long eventSubscriptionId)
        {
            var existingState = await GetAsync(eventSubscriptionId.ToString(), eventSubscriptionId.ToString());
            if(existingState == null)
            {
                existingState = new EventSubscriptionStateEntity
                {
                    EventSubscriptionId = eventSubscriptionId,
                    Id = eventSubscriptionId
                };
            }
            return existingState;
        }

        public static EventSubscriptionStateEntity Map(IEventSubscriptionStateDto dto)
        {
            if(dto is EventSubscriptionStateEntity e) return e;

            return new EventSubscriptionStateEntity
            {
                EventSubscriptionId = dto.EventSubscriptionId,
                Id = dto.Id == 0 ? dto.EventSubscriptionId : dto.Id,
                Values = dto.Values
            };
        }

        public async Task UpsertAsync(IEnumerable<IEventSubscriptionStateDto> state)
        {
            foreach(var item in state)
            {
                var entity = Map(item);
                await base.UpsertAsync(entity);
            }
        }
    }
}
