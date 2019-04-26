using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class EventSubscriptionAddressRepository : OwnedRepository<IEventSubscriptionAddressDto, EventSubscriptionAddressEntity>, IEventSubscriptionAddressRepository
    {
        public EventSubscriptionAddressRepository(CloudTable table) : base(table)
        {
        }

        protected override EventSubscriptionAddressEntity Map(IEventSubscriptionAddressDto dto)
        {
            return new EventSubscriptionAddressEntity
            {
                Id = dto.Id,
                EventSubscriptionId = dto.EventSubscriptionId,
                Address = dto.Address
            };
        }
    }
}
