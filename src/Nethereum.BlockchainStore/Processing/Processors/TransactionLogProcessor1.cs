using Nethereum.BlockchainStore.Repositories;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class TransactionLogProcessor : ILogProcessor
    {
        public TransactionLogProcessor(ITransactionLogRepository repository, Predicate<FilterLog> predicate = null)
        {
            Repository = repository;
            Predicate = predicate ?? new Predicate<FilterLog>(l => true);
        }

        public ITransactionLogRepository Repository { get; }
        public Predicate<FilterLog> Predicate { get; }

        public virtual bool IsLogForEvent(FilterLog log) => Predicate(log);

        public virtual async Task ProcessLogsAsync(params FilterLog[] eventLogs)
        {
            foreach(var log in eventLogs)
            {
                if (Predicate(log))
                {
                    await Repository.UpsertAsync(log);
                }
            }
        }
    }
}
