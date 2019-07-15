using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class SubscriberQueueRepository :
        SubscriberOwnedRepository<ISubscriberQueueDto, SubscriberQueueEntity>,
        ISubscriberQueueRepository
    {
        public SubscriberQueueRepository(CloudTable table) : base(table)
        {
        }

        protected override SubscriberQueueEntity Map(ISubscriberQueueDto dto)
        {
            return new SubscriberQueueEntity
            {
                Disabled = dto.Disabled,
                Id = dto.Id,
                Name = dto.Name,
                SubscriberId = dto.SubscriberId
            };
        }
    }
}
