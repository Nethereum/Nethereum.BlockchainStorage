using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Storage.Entities;
using Nethereum.BlockchainProcessing.Storage.Repositories;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using System.Threading.Tasks;
using TransactionLog = Nethereum.BlockchainStore.AzureTables.Entities.TransactionLog;

namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class TransactionLogRepository : AzureTableRepository<TransactionLog>, ITransactionLogRepository
    {
        public TransactionLogRepository(CloudTable table) : base(table)
        {
        }

        public async Task<ITransactionLogView> FindByTransactionHashAndLogIndexAsync(string transactionHash, BigInteger logIndex)
        {
            var operation = TableOperation.Retrieve<TransactionLog>(transactionHash, logIndex.ToString());
            var results = await Table.ExecuteAsync(operation).ConfigureAwait(false);
            return results.Result as TransactionLog;
        }

        public async Task UpsertAsync(FilterLogVO log)
        {
            var entity = TransactionLog.CreateTransactionLog(log.Log);

            await UpsertAsync(entity).ConfigureAwait(false);
        }
    }
}