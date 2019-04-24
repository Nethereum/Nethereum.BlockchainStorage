using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class AzureTableRepository<TEntity> where TEntity: class, ITableEntity, new()
    {
        protected readonly CloudTable Table;

        public AzureTableRepository(CloudTable table)
        {
            this.Table = table;
        }

        public virtual async Task<TEntity> GetAsync(string partitionKey, string rowKey = null)
        {
            rowKey = rowKey ?? string.Empty;
            var operation = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);
            var results = await Table.ExecuteAsync(operation);
            return results.Result as TEntity;
        }

        public virtual async Task<TEntity[]> GetManyAsync(string partitionKey)
        {
            string filter = TableQuery.GenerateFilterCondition(
                    "PartitionKey", QueryComparisons.Equal, partitionKey);

            var query = new TableQuery<TEntity>().Where(filter);

            var entities = new List<TEntity>();

            TableContinuationToken continuationToken = null;

            do
            {
                var result = await Table.ExecuteQuerySegmentedAsync(query, continuationToken);
                entities.AddRange(result);
            }
            while (continuationToken != null);

            return entities.ToArray();
        }

        public virtual async Task<object> UpsertAsync(ITableEntity entity)
        {
            return await UpsertAsync(entity, Table);
        }

        public virtual async Task<object> UpsertAsync(ITableEntity entity, CloudTable table)
        {
            var upsertOperation = TableOperation.InsertOrReplace(entity);
            var result = await table.ExecuteAsync(upsertOperation).ConfigureAwait(false);
            return result.Result;
        }

        public virtual async Task<object> DeleteAsync(ITableEntity entity)
        {
            var deleteOperation = TableOperation.Delete(entity);
            var result = await Table.ExecuteAsync(deleteOperation).ConfigureAwait(false);
            return result.Result;
        }

        protected virtual string CreatePartitionKeyFilterCondition(string partitionKey)
        {
            return TableQuery.GenerateFilterCondition("PartitionKey",
                QueryComparisons.Equal, partitionKey);
        }
    }
}
