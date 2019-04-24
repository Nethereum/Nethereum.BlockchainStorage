using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class EventSubscriptionAddressRepository : AzureTableRepository<EventSubscriptionAddressEntity>, IEventSubscriptionAddressRepository
    {
        public EventSubscriptionAddressRepository(CloudTable table) : base(table)
        {
        }

        public async Task<IEventSubscriptionAddressDto[]> GetEventSubscriptionAddressesAsync(long eventSubscriptionId)
        {
            return await GetManyAsync(eventSubscriptionId.ToString());
        }

        public static EventSubscriptionAddressEntity Map(IEventSubscriptionAddressDto dto)
        {
            if (dto is EventSubscriptionAddressEntity entity) return entity;

            return new EventSubscriptionAddressEntity
            {
                Id = dto.Id,
                EventSubscriptionId = dto.EventSubscriptionId,
                Address = dto.Address
            };
        }

        public async Task<IEventSubscriptionAddressDto> UpsertAsync(IEventSubscriptionAddressDto dto)
        {
            var entity = Map(dto);
            return await base.UpsertAsync(entity) as IEventSubscriptionAddressDto;
        }
    }
}
