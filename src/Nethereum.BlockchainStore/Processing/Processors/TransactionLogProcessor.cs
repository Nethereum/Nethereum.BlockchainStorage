using Nethereum.BlockchainStore.Repositories;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class TransactionLogProcessor<TEventDto> : ILogProcessor where TEventDto : class, new()
    {
        public TransactionLogProcessor(ITransactionLogRepository repository, Predicate<EventLog<TEventDto>> predicate = null)
        {
            Repository = repository;
            Predicate = predicate ?? new Predicate<EventLog<TEventDto>>(l => true);
        }

        public ITransactionLogRepository Repository { get; }
        public Predicate<EventLog<TEventDto>> Predicate { get; }

        public virtual bool IsLogForEvent(FilterLog log) => log.IsLogForEvent<TEventDto>();

        public virtual async Task ProcessLogsAsync(params FilterLog[] eventLogs)
        {
            var decoded = eventLogs.DecodeAllEventsIgnoringIndexMisMatches<TEventDto>();

            foreach (var log in decoded)
            {
                if (Predicate(log))
                {
                    await Repository.UpsertAsync(log.Log);
                }
            }
        }
    }
}
