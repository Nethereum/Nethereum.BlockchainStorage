using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.Repositories;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Nethereum.RPC.Eth.DTOs;
using TransactionLog = Nethereum.BlockchainStore.AzureTables.Entities.TransactionLog;
using Nethereum.BlockchainProcessing.Handlers;

namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class TransactionLogRepository : AzureTableRepository<TransactionLog>, ITransactionLogRepository, ILogHandler
    {
        public TransactionLogRepository(CloudTable table) : base(table)
        {
        }

        public async Task<ITransactionLogView> FindByTransactionHashAndLogIndexAsync(string transactionHash, long logIndex)
        {
            var operation = TableOperation.Retrieve<TransactionLog>(transactionHash, logIndex.ToString());
            var results = await Table.ExecuteAsync(operation).ConfigureAwait(false);
            return results.Result as TransactionLog;
        }

        public Task HandleAsync(FilterLog log) => UpsertAsync(log);

        public async Task UpsertAsync(FilterLog log)
        {
            var entity = TransactionLog.CreateTransactionLog(log);

            await UpsertAsync(entity).ConfigureAwait(false);
        }
    }
}