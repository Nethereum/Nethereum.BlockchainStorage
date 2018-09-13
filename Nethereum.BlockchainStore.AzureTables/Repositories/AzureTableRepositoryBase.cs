using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class AzureTableRepository<TEntity> where TEntity: ITableEntity
    {
        protected readonly CloudTable _table;

        public AzureTableRepository(CloudTable table)
        {
            this._table = table;
        }

        public async Task<object> UpsertAsync(ITableEntity entity)
        {
            var upsertOperation = TableOperation.InsertOrReplace(entity);
            var result = await _table.ExecuteAsync(upsertOperation).ConfigureAwait(false);
            return result.Result;
        }

        public async Task<object> DeleteAsync(ITableEntity entity)
        {
            var deleteOperation = TableOperation.Delete(entity);
            var result = await _table.ExecuteAsync(deleteOperation).ConfigureAwait(false);
            return result.Result;
        }

        protected string CreatePartionKeyFilterCondition(string partitionKey)
        {
            return TableQuery.GenerateFilterCondition("PartitionKey",
                QueryComparisons.Equal, partitionKey);
        }
    }
}
