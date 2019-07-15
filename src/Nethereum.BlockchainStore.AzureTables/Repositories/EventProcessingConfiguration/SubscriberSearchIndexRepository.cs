using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class SubscriberSearchIndexRepository :
        SubscriberOwnedRepository<ISubscriberSearchIndexDto, SubscriberSearchIndexEntity>,
        ISubscriberSearchIndexRepository
    {
        public SubscriberSearchIndexRepository(CloudTable table) : base(table)
        {
        }

        protected override SubscriberSearchIndexEntity Map(ISubscriberSearchIndexDto dto)
        {
            return new SubscriberSearchIndexEntity
            {
                Disabled = dto.Disabled,
                Id = dto.Id,
                Name = dto.Name,
                SubscriberId = dto.SubscriberId
            };
        }
    }
}
