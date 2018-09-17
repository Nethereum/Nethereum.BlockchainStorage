﻿using System.Threading.Tasks;
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

        public async Task<ITransactionLogView> FindByTransactionHashAndLogIndexAsync(string hash, long idx)
        {
            using (var context = _contextFactory.CreateContext())
            {
                return await context.TransactionLogs.FindByTransactionHashAndLogIndexAsync(hash, idx).ConfigureAwait(false);
            }
        }

        public async Task UpsertAsync(string transactionHash, long logIndex, JObject log)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var transactionLog = await context.TransactionLogs.FindByTransactionHashAndLogIndexAsync(transactionHash, logIndex).ConfigureAwait(false) 
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
