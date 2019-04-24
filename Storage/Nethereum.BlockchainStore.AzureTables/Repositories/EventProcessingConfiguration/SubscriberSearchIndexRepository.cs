using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class SubscriberSearchIndexRepository : AzureTableRepository<SubscriberSearchIndexEntity>, ISubscriberSearchIndexRepository
    {
        public SubscriberSearchIndexRepository(CloudTable table) : base(table)
        {
        }

        public async Task<ISubscriberSearchIndexDto> GetAsync(long subscriberId, long searchIndexId)
        {
            return await GetAsync(subscriberId.ToString(), searchIndexId.ToString());
        }

        public static SubscriberSearchIndexEntity Map(ISubscriberSearchIndexDto dto)
        {
            if (dto is SubscriberSearchIndexEntity e) return e;

            return new SubscriberSearchIndexEntity
            {
                Disabled = dto.Disabled,
                Id = dto.Id,
                Name = dto.Name,
                SubscriberId = dto.SubscriberId
            };
        }

        public async Task<ISubscriberSearchIndexDto> UpsertAsync(ISubscriberSearchIndexDto dto)
        {
            var entity = Map(dto);
            return await base.UpsertAsync(entity) as ISubscriberSearchIndexDto;
        }
    }
}
