using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.AzureTables.Entities;
using Nethereum.BlockchainStore.Repositories;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class TransactionLogRepository : AzureTableRepository<TransactionLog>, ITransactionLogRepository
    {
        public TransactionLogRepository(CloudTable table) : base(table)
        {
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