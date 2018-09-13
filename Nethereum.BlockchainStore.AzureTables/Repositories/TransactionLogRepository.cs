using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.AzureTables.Entities;
using Nethereum.BlockchainStore.Repositories;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class TransactionLogRepository : AzureTableRepository<TransactionLog>, IAzureTableTransactionLogRepository
    {
        public TransactionLogRepository(CloudTable table) : base(table)
        {
        }

        public async Task<TransactionLog> FindByTransactionHashAndLogIndexAsync(string transactionHash, long logIndex)
        {
            var operation = TableOperation.Retrieve<TransactionLog>(transactionHash, logIndex.ToString());
            var results = await Table.ExecuteAsync(operation).ConfigureAwait(false);
            return results.Result as TransactionLog;
        }

        public async Task UpsertAsync(string transactionHash, long logIndex,
            JObject log)
        {
            var entity = TransactionLog.CreateTransactionLog(transactionHash,
                logIndex, log);

            await UpsertAsync(entity).ConfigureAwait(false);
        }
    }
}