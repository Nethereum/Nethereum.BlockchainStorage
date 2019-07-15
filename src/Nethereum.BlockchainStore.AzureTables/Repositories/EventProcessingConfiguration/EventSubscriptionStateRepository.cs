using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class EventSubscriptionStateRepository : OneToOneRepository<IEventSubscriptionStateDto, EventSubscriptionStateEntity>, IEventSubscriptionStateRepository
    {
        public EventSubscriptionStateRepository(CloudTable table) : base(table)
        {
        }

        public override async Task<IEventSubscriptionStateDto> GetAsync(long eventSubscriptionId)
        {
            var existingState = await GetAsync(eventSubscriptionId.ToString(), eventSubscriptionId.ToString()).ConfigureAwait(false);
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

        protected override  EventSubscriptionStateEntity Map(IEventSubscriptionStateDto dto)
        {
            return new EventSubscriptionStateEntity
            {
                EventSubscriptionId = dto.EventSubscriptionId,
                Id = dto.Id == 0 ? dto.EventSubscriptionId : dto.Id,
                Values = dto.Values
            };
        }
    }
}
