using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.Entities;
using Newtonsoft.Json.Linq;
using Wintellect.Azure.Storage.Table;

namespace Nethereum.BlockchainStore.Repositories
{
    public class TransactionLogRepository : ITransactionLogRepository
    {
        protected AzureTable Table { get; set; }

        public TransactionLogRepository(CloudTable cloudTable)
        {
            Table = new AzureTable(cloudTable);
        }

        public async Task UpsertAsync(string transactionHash, long logIndex,
            JObject log)
        {
            var entity = TransactionLog.CreateTransactionLog(Table, transactionHash,
                logIndex, log);
            await entity.InsertOrReplaceAsync().ConfigureAwait(false);
        }
    }
}