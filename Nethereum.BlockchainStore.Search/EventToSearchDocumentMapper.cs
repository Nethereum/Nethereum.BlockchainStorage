using System;
using Nethereum.Contracts;

namespace Nethereum.BlockchainStore.Search
{
    public class EventToSearchDocumentMapper<TEvent, TSearchDocument> : IEventToSearchDocumentMapper<TEvent, TSearchDocument>
        where TEvent : class where TSearchDocument : class
    {
        private readonly Func<EventLog<TEvent>, TSearchDocument> _mappingFunc;

        public EventToSearchDocumentMapper(Func<EventLog<TEvent>, TSearchDocument> mappingFunc)
        {
            _mappingFunc = mappingFunc;
        }

        public TSearchDocument Map(EventLog<TEvent> from)
        {
            return _mappingFunc.Invoke(from);
        }
    }
}