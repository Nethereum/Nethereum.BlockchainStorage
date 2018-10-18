using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.EF.Repositories
{
    public class TransactionLogRepository : RepositoryBase, ITransactionLogRepository
    {
        public TransactionLogRepository(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        public async Task<ITransactionLogView> FindByTransactionHashAndLogIndexAsync(string hash, long idx)
        {
            using (var context = _contextFactory.CreateContext())
            {
                return await context.TransactionLogs.FindByTransactionHashAndLogIndex(hash, idx).ConfigureAwait(false);
            }
        }

        public async Task UpsertAsync(Log log)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var transactionLog = await context.TransactionLogs.FindByTransactionHashAndLogIndex(log.TransactionHash, log.LogIndex.ToLong()).ConfigureAwait(false) 
                          ?? new TransactionLog();

                transactionLog.Map(log);
                transactionLog.UpdateRowDates();

                context.TransactionLogs.AddOrUpdate(transactionLog);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }


    }
}
