using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class SubscriberContractsRepository : 
        SubscriberOwnedRepository<ISubscriberContractDto, SubscriberContractEntity>, 
        ISubscriberContractRepository
    {
        public SubscriberContractsRepository(CloudTable table) : base(table)
        {
        }

        protected override SubscriberContractEntity Map(ISubscriberContractDto dto)
        {
            if (dto is SubscriberContractEntity entity) return entity;

            return new SubscriberContractEntity
            {
                Id = dto.Id,
                SubscriberId = dto.SubscriberId,
                Abi = dto.Abi,
                Name = dto.Name
            };
        }
    }
}
