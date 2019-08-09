using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processor;
using Nethereum.Contracts;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{
    public class EventLogProcessor<TEventDTO> : EventLogProcessorHandler<TEventDTO> where TEventDTO : class, IEventDTO, new()
    {
        public EventLogProcessor(
            IIndexer<EventLog<TEventDTO>> eventIndexer) :
                base((eventLog) => eventIndexer.IndexAsync(eventLog))
        {
            EventIndexer = eventIndexer;
        }

        public EventLogProcessor(
            IIndexer<EventLog<TEventDTO>> eventIndexer,
            Func<EventLog<TEventDTO>, Task<bool>> eventCriteria) :
                base((eventLog) => eventIndexer.IndexAsync(eventLog), eventCriteria)
        {
            EventIndexer = eventIndexer;
        }

        public EventLogProcessor(
            IIndexer<EventLog<TEventDTO>> eventIndexer,
            Func<EventLog<TEventDTO>, bool> eventCriteria) :
                base((eventLog) => eventIndexer.IndexAsync(eventLog), eventCriteria)
        {
            EventIndexer = eventIndexer;
        }

        public IIndexer<EventLog<TEventDTO>> EventIndexer { get; }
    }
}
