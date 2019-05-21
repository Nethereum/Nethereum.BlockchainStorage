using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class EventLogQueueProcessor : ILogProcessor
    {
        public EventLogQueueProcessor(IQueue destinationQueue, Predicate<FilterLog> predicate = null, Func<FilterLog, object> mapper = null)
        {
            Mapper = mapper;
            DestinationQueue = destinationQueue;
            Predicate = predicate ?? new Predicate<FilterLog>((l) => true);
        }

        public IQueue DestinationQueue { get; }
        public Predicate<FilterLog> Predicate { get; }
        public Func<FilterLog, object> Mapper { get; }

        public virtual bool IsLogForEvent(FilterLog log) => Predicate(log);

        public virtual async Task ProcessLogsAsync(params FilterLog[] eventLogs)
        {
            foreach (var eventLog in eventLogs)
            {
                if(Predicate(eventLog))
                { 
                    await DestinationQueue.AddMessageAsync(Mapper?.Invoke(eventLog) ?? eventLog).ConfigureAwait(false);
                }
            }
        }
    }
}
