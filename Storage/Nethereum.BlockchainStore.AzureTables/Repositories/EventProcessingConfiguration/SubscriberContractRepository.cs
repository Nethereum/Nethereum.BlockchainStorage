using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class SubscriberContractsRepository : AzureTableRepository<SubscriberContractEntity>, ISubscriberContractsRepository
    {
        public SubscriberContractsRepository(CloudTable table) : base(table)
        {
        }

        public async Task<ISubscriberContractDto> GetContractAsync(long subscriberId, long contractId) 
        { 
            return await GetAsync(subscriberId.ToString(), contractId.ToString());
        }

        public static SubscriberContractEntity Map(ISubscriberContractDto dto)
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

        public async Task<ISubscriberContractDto> UpsertAsync(ISubscriberContractDto contract)
        {
            var entity = Map(contract);
            return await base.UpsertAsync(entity) as ISubscriberContractDto;
        }
    }
}
