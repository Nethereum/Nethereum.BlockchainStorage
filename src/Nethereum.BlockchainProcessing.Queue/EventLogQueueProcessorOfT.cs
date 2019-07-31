using Nethereum.BlockchainProcessing.Processor;
using Nethereum.Contracts;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Queue
{
    public class EventLogQueueProcessor<TEventDto> : EventLogProcessorHandler<TEventDto> where TEventDto : class, new()
    {
        public EventLogQueueProcessor(
            IQueue destinationQueue, 
            Func<EventLog<TEventDto>, object> mapper = null) : 
                base((eventLog) => destinationQueue.AddMessageAsync(mapper?.Invoke(eventLog) ?? eventLog))
        {

        }

        public EventLogQueueProcessor(
            IQueue destinationQueue,  
            Func<EventLog<TEventDto>, Task<bool>> eventCriteria,
            Func<EventLog<TEventDto>, object> mapper = null) : 
                base((eventLog) => destinationQueue.AddMessageAsync(mapper?.Invoke(eventLog) ?? eventLog), eventCriteria)
        {

        }

        public EventLogQueueProcessor(
            IQueue destinationQueue,
            Func<EventLog<TEventDto>, bool> eventCriteria,
            Func<EventLog<TEventDto>, object> mapper = null) :
                base((eventLog) => destinationQueue.AddMessageAsync(mapper?.Invoke(eventLog) ?? eventLog), eventCriteria)
        {

        }
    }
}
