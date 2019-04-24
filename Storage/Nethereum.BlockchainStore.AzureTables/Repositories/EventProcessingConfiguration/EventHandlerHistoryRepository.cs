using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class EventHandlerHistoryRepository : AzureTableRepository<EventHandlerHistoryEntity>, IEventHandlerHistoryRepository
    {
        public EventHandlerHistoryRepository(CloudTable table) : base(table)
        {
        }

        public static EventHandlerHistoryEntity Map(IEventHandlerHistoryDto dto)
        {
            if(dto is EventHandlerHistoryEntity e) return e;

            return new EventHandlerHistoryEntity
            {
                EventHandlerId = dto.EventHandlerId,
                EventKey = dto.EventKey,
                EventSubscriptionId = dto.EventSubscriptionId,
                SubscriberId = dto.SubscriberId
            };
        }

        public async Task<IEventHandlerHistoryDto> AddAsync(IEventHandlerHistoryDto dto) 
        {
            var entity = Map(dto);
            return await base.UpsertAsync(entity) as IEventHandlerHistoryDto;
        }

        public async Task<bool> ContainsAsync(long eventHandlerId, string eventKey)
        {
            return await GetAsync(eventHandlerId, eventKey) != null;
        }

        public async Task<IEventHandlerHistoryDto> GetAsync(long eventHandlerId, string eventKey)
        {
            return await GetAsync(eventHandlerId.ToString(), eventKey);
        }

        public async Task<IEventHandlerHistoryDto[]> GetAsync(long eventHandlerId)
        {
            return await GetManyAsync(eventHandlerId.ToString());
        }
    }
}
