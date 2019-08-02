﻿using Nethereum.BlockchainProcessing.Processor;
using Nethereum.Contracts;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{
    public class EventProcessor<TEventDTO> : EventLogProcessorHandler<TEventDTO> where TEventDTO : class, new()
    {
        public EventProcessor(
            IIndexer<EventLog<TEventDTO>> eventIndexer) :
                base((eventLog) => eventIndexer.IndexAsync(eventLog))
        {

        }

        public EventProcessor(
            IIndexer<EventLog<TEventDTO>> eventIndexer,
            Func<EventLog<TEventDTO>, Task<bool>> eventCriteria) :
                base((eventLog) => eventIndexer.IndexAsync(eventLog), eventCriteria)
        {

        }

        public EventProcessor(
            IIndexer<EventLog<TEventDTO>> eventIndexer,
            Func<EventLog<TEventDTO>, bool> eventCriteria) :
                base((eventLog) => eventIndexer.IndexAsync(eventLog), eventCriteria)
        {

        }
    }
}
