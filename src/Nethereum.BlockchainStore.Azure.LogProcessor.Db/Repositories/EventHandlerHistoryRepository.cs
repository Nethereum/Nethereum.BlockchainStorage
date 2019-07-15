using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class EventHandlerHistoryRepository : OwnedRepository<IEventHandlerHistoryDto, EventHandlerHistoryEntity>, IEventHandlerHistoryRepository
    {
        public EventHandlerHistoryRepository(CloudTable table) : base(table)
        {
        }

        protected override EventHandlerHistoryEntity Map(IEventHandlerHistoryDto dto)
        {
            return new EventHandlerHistoryEntity
            {
                EventHandlerId = dto.EventHandlerId,
                EventKey = dto.EventKey,
                EventSubscriptionId = dto.EventSubscriptionId,
                SubscriberId = dto.SubscriberId
            };
        }

        public async Task<bool> ContainsAsync(long eventHandlerId, string eventKey)
        {
            return await GetAsync(eventHandlerId, eventKey).ConfigureAwait(false) != null;
        }

        public async Task<IEventHandlerHistoryDto> GetAsync(long eventHandlerId, string eventKey)
        {
            return await GetAsync(eventHandlerId.ToString(), eventKey).ConfigureAwait(false);
        }

    }
}
