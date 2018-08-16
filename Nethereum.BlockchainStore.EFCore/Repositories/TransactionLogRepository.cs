using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Repositories;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.EFCore.Repositories
{
    public class TransactionLogRepository : RepositoryBase, ITransactionLogRepository
    {
        public TransactionLogRepository(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        public async Task UpsertAsync(string transactionHash, long logIndex, JObject log)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var transactionLog = await context.TransactionLogs.FindByTransactionHashAndLogIndex(transactionHash, logIndex).ConfigureAwait(false) 
                          ?? new TransactionLog();

                transactionLog.Map(transactionHash, logIndex, log);

                transactionLog.UpdateRowDates();

                if (transactionLog.IsNew())
                    context.TransactionLogs.Add(transactionLog);
                else
                    context.TransactionLogs.Update(transactionLog);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
