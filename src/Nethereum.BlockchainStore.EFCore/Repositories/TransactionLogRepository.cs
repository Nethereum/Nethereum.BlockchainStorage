using Nethereum.BlockchainProcessing.Storage.Entities;
using Nethereum.BlockchainProcessing.Storage.Entities.Mapping;
using Nethereum.BlockchainProcessing.Storage.Repositories;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.EFCore.Repositories
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
                return await context.TransactionLogs.FindByTransactionHashAndLogIndexAsync(hash, idx).ConfigureAwait(false);
            }
        }

        public async Task UpsertAsync(FilterLog log)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var transactionLog = await context.TransactionLogs.FindByTransactionHashAndLogIndexAsync(log.TransactionHash, log.LogIndex.ToLong()).ConfigureAwait(false) 
                          ?? new TransactionLog();

                transactionLog.Map(log);
                transactionLog.UpdateRowDates();

                if (transactionLog.IsNew())
                    context.TransactionLogs.Add(transactionLog);
                else
                    context.TransactionLogs.Update(transactionLog);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task UpsertAsync(FilterLogVO log)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var transactionLog = await context.TransactionLogs.FindByTransactionHashAndLogIndexAsync(log.Log.TransactionHash, log.Log.LogIndex.ToLong()).ConfigureAwait(false)
                          ?? new TransactionLog();

                transactionLog.MapToStorageEntityForUpsert(log);

                if (transactionLog.IsNew())
                    context.TransactionLogs.Add(transactionLog);
                else
                    context.TransactionLogs.Update(transactionLog);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
