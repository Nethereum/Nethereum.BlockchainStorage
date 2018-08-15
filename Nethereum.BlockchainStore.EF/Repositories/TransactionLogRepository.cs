using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Repositories;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.EF.Repositories
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

                MapValues(transactionHash, logIndex, log, transactionLog);

                transactionLog.UpdateRowDates();

                context.TransactionLogs.AddOrUpdate(transactionLog);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private void MapValues(string transactionHash, long logIndex, JObject log, TransactionLog transactionLog)
        {
            transactionLog.TransactionHash = transactionHash;
            transactionLog.LogIndex = logIndex;
            transactionLog.Address = log["address"].Value<string>() ?? string.Empty;
            transactionLog.Data = log["data"].Value<string>() ?? string.Empty;
            var topics = log["topics"] as JArray;

            if (topics?.Count > 0)
            {
                transactionLog.EventHash = topics[0].Value<string>();

                if (topics.Count > 1)
                    transactionLog.IndexVal1 = topics[1].Value<string>();

                if (topics.Count > 2)
                    transactionLog.IndexVal2 = topics[2].Value<string>();

                if (topics.Count > 3)
                    transactionLog.IndexVal3 = topics[3].Value<string>();
            }
        }
    }
}
