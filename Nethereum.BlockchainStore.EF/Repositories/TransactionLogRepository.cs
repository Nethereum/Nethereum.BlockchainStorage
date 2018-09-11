using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Repositories;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.EF.Repositories
{
    public class TransactionLogRepository : RepositoryBase, IEntityTransactionLogRepository
    {
        public TransactionLogRepository(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        public async Task<TransactionLog> FindByTransactionHashAndLogIndexAsync(string hash, long idx)
        {
            using (var context = _contextFactory.CreateContext())
            {
                return await context.TransactionLogs.FindByTransactionHashAndLogIndex(hash, idx).ConfigureAwait(false);
            }
        }

        public async Task UpsertAsync(string transactionHash, long logIndex, JObject log)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var transactionLog = await context.TransactionLogs.FindByTransactionHashAndLogIndex(transactionHash, logIndex).ConfigureAwait(false) 
                          ?? new TransactionLog();

                transactionLog.Map(transactionHash, logIndex, log);
                transactionLog.UpdateRowDates();

                context.TransactionLogs.AddOrUpdate(transactionLog);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }


    }
}
