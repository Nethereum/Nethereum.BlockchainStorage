using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;
using System.Collections.Generic;
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
            string filter = TableQuery.GenerateFilterCondition(
                    "PartitionKey", QueryComparisons.Equal, partitionId.ToString());

            var subscriberQuery = new TableQuery<SubscriberEntity>().Where(filter);

            var subscriberDtos = new List<ISubscriberDto>();

            TableContinuationToken continuationToken = null;

            do
            {
                var result = await Table.ExecuteQuerySegmentedAsync(subscriberQuery, continuationToken);
                subscriberDtos.AddRange(result);
            }
            while (continuationToken != null);

            return subscriberDtos.ToArray();
        }

        public SubscriberEntity Map(ISubscriberDto dto)
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
