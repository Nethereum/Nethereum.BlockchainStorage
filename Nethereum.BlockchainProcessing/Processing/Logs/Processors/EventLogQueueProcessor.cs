using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class EventLogQueueProcessor<TEventDto> : LogProcessorBase<TEventDto> where TEventDto : class, new()
    {
        public EventLogQueueProcessor(IQueue destinationQueue, Predicate<EventLog<TEventDto>> predicate = null, Func<EventLog<TEventDto>, object> mapper = null)
        {
            DestinationQueue = destinationQueue;
            Predicate = predicate ?? new Predicate<EventLog<TEventDto>>((e) => true);
            Mapper = mapper;
        }

        public IQueue DestinationQueue { get; }
        public Predicate<EventLog<TEventDto>> Predicate { get; }
        public Func<EventLog<TEventDto>, object> Mapper { get; }

        public override async Task ProcessLogsAsync(params FilterLog[] eventLogs)
        {
            var decoded = eventLogs.DecodeAllEventsIgnoringIndexMisMatches<TEventDto>();

            foreach(var eventLog in decoded)
            {
                if(!Predicate(eventLog)) continue;

                await DestinationQueue.AddMessageAsync(Mapper?.Invoke(eventLog) ?? eventLog);
            }
        }
    }
}
