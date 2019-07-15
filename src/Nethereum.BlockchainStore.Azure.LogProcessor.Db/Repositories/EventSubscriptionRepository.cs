using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class EventSubscriptionRepository : OwnedRepository<IEventSubscriptionDto, EventSubscriptionEntity>,  IEventSubscriptionRepository
    {
        public EventSubscriptionRepository(CloudTable table) : base(table)
        {
        }

        protected override EventSubscriptionEntity Map(IEventSubscriptionDto dto)
        {
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
    }
}
