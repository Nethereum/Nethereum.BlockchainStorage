using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class SubscriberStorageRepository :
        SubscriberOwnedRepository<ISubscriberStorageDto, SubscriberStorageEntity>, ISubscriberStorageRepository
    {
        public SubscriberStorageRepository(CloudTable table) : base(table)
        {
        }

        protected override SubscriberStorageEntity Map(ISubscriberStorageDto dto)
        {
            return new SubscriberStorageEntity
            {
                Disabled = dto.Disabled,
                Id = dto.Id,
                Name = dto.Name,
                SubscriberId = dto.SubscriberId
            };
        }
    }
}
