using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.RPC.Eth.DTOs;
using System.Data.Entity.Migrations;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.EF.Repositories
{
    public class TransactionLogRepository : RepositoryBase, ITransactionLogRepository
    {
        public TransactionLogRepository(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        public async Task<ITransactionLogView> FindByTransactionHashAndLogIndexAsync(string hash, BigInteger idx)
        {
            using (var context = _contextFactory.CreateContext())
            {
                return await context.TransactionLogs.FindByTransactionHashAndLogIndex(hash, idx).ConfigureAwait(false);
            }
        }

        public async Task UpsertAsync(FilterLogVO log)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var transactionLog = await context.TransactionLogs.FindByTransactionHashAndLogIndex(log.Log.TransactionHash, log.Log.LogIndex.ToLong()).ConfigureAwait(false)
                          ?? new TransactionLog();

                transactionLog.MapToStorageEntityForUpsert(log);

                context.TransactionLogs.AddOrUpdate(transactionLog);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
