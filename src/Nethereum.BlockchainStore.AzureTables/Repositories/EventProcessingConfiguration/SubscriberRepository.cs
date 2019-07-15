using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{

    public class SubscriberRepository : OwnedRepository<ISubscriberDto, SubscriberEntity>, ISubscriberRepository
    {
        public SubscriberRepository(CloudTable table) : base(table)
        {
        }

        protected override SubscriberEntity Map(ISubscriberDto dto)
        {
            return new SubscriberEntity
            {
                Id = dto.Id,
                PartitionId = dto.PartitionId,
                Disabled = dto.Disabled,
                Name = dto.Name
            };
        }
    }
}
