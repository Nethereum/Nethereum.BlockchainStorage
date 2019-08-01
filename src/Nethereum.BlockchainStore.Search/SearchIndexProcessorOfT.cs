using Nethereum.BlockchainProcessing.Processor;
using Nethereum.Contracts;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{
    public class SearchIndexProcessor<TEventDto> : EventLogProcessorHandler<TEventDto> where TEventDto : class, new()
    {
        public SearchIndexProcessor(
            IIndexer<EventLog<TEventDto>> eventIndexer) :
                base((eventLog) => eventIndexer.IndexAsync(eventLog))
        {

        }

        public SearchIndexProcessor(
            IIndexer<EventLog<TEventDto>> eventIndexer,
            Func<EventLog<TEventDto>, Task<bool>> eventCriteria) :
                base((eventLog) => eventIndexer.IndexAsync(eventLog), eventCriteria)
        {

        }

        public SearchIndexProcessor(
            IIndexer<EventLog<TEventDto>> eventIndexer,
            Func<EventLog<TEventDto>, bool> asyncEventCriteria) :
                base((eventLog) => eventIndexer.IndexAsync(eventLog), asyncEventCriteria)
        {

        }
    }
}
