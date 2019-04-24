using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class EventSubscriptionRepository : AzureTableRepository<EventSubscriptionEntity>,  IEventSubscriptionRepository
    {
        public EventSubscriptionRepository(CloudTable table) : base(table)
        {
        }

        public async Task<IEventSubscriptionDto[]> GetEventSubscriptionsAsync(long subscriberId)
        {
           return await GetManyAsync(subscriberId.ToString());
        }

        public static EventSubscriptionEntity Map(IEventSubscriptionDto dto)
        {
            if (dto is EventSubscriptionEntity entity) return entity;

            return new EventSubscriptionEntity
            {
                Id = dto.Id,
                SubscriberId = dto.SubscriberId,
                CatchAllContractEvents = dto.CatchAllContractEvents,
                ContractId = dto.ContractId,
                Disabled = dto.Disabled,
                EventSignatures = dto.EventSignatures
            };
        }

        public async Task<IEventSubscriptionDto> UpsertAsync(IEventSubscriptionDto subscription)
        {
            var entity = Map(subscription);
            return await base.UpsertAsync(entity) as IEventSubscriptionDto;
        }
    }
}
