using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{

    public class SubscriberRepository : AzureTableRepository<SubscriberEntity>, ISubscriberRepository
    {
        public SubscriberRepository(CloudTable table) : base(table)
        {
        }

        public async Task<ISubscriberDto[]> GetSubscribersAsync(long partitionId)
        {
            var entities = await GetManyAsync(partitionId.ToString());
            return entities;
        }

        public static SubscriberEntity Map(ISubscriberDto dto)
        {
            if(dto is SubscriberEntity entity) return entity;

            return new SubscriberEntity
            {
                Id = dto.Id,
                PartitionId = dto.PartitionId,
                Disabled = dto.Disabled,
                Name = dto.Name
            };
        }

        public async Task<ISubscriberDto> UpsertAsync(ISubscriberDto subscriber)
        {
            var result = await base.UpsertAsync(Map(subscriber));
            return result as ISubscriberDto;
        }
    }
}
