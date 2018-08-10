using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.SqlServer.Entities;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.SqlServer.Repositories
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
                          ?? new Entities.TransactionLog();

                MapValues(transactionHash, logIndex, log, transactionLog);

                transactionLog.UpdateRowDates();

                if (transactionLog.IsNew())
                    context.TransactionLogs.Add(transactionLog);
                else
                    context.TransactionLogs.Update(transactionLog);

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
            if (topics != null)
            {
                transactionLog.Topics = topics.ToString();
                if (topics.Count > 0)
                    transactionLog.Topic0 = topics[0].ToString();
            }
        }
    }
}
