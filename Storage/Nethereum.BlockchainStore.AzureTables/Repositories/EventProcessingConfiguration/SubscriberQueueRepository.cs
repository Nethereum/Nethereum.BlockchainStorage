using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class SubscriberQueueRepository : AzureTableRepository<SubscriberQueueEntity>, ISubscriberQueueRepository
    {
        public SubscriberQueueRepository(CloudTable table) : base(table)
        {
        }

        public async Task<ISubscriberQueueDto> GetAsync(long subscriberId, long queueId)
        {
            return await GetAsync(subscriberId.ToString(), queueId.ToString());
        }

        public static SubscriberQueueEntity Map(ISubscriberQueueDto dto)
        {
            if(dto is SubscriberQueueEntity e) return e;

            return new SubscriberQueueEntity
            {
                Disabled = dto.Disabled,
                Id = dto.Id,
                Name = dto.Name,
                SubscriberId = dto.SubscriberId
            };
        }

        public async Task<ISubscriberQueueDto> UpsertAsync(ISubscriberQueueDto dto)
        {
            var entity = Map(dto);
            return await base.UpsertAsync(entity) as ISubscriberQueueDto;
        }
    }

    public class SubscriberStorageRepository : AzureTableRepository<SubscriberStorageEntity>, ISubscriberStorageRepository
    {
        public SubscriberStorageRepository(CloudTable table) : base(table)
        {
        }

        public async Task<ISubscriberStorageDto> GetAsync(long subscriberId, long storageRepoId)
        {
            return await GetAsync(subscriberId.ToString(), storageRepoId.ToString());
        }

        public async Task<ISubscriberStorageDto[]> GetAsync(long subscriberId)
        {
            return await GetManyAsync(subscriberId.ToString());
        }

        public static SubscriberStorageEntity Map(ISubscriberStorageDto dto)
        {
            if (dto is SubscriberStorageEntity e) return e;

            return new SubscriberStorageEntity
            {
                Disabled = dto.Disabled,
                Id = dto.Id,
                Name = dto.Name,
                SubscriberId = dto.SubscriberId
            };
        }

        public async Task<ISubscriberStorageDto> UpsertAsync(ISubscriberStorageDto dto)
        {
            var entity = Map(dto);
            return await base.UpsertAsync(entity) as ISubscriberStorageDto;
        }
    }
}
