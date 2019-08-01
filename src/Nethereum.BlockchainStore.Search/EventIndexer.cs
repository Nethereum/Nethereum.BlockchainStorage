using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;

namespace Nethereum.BlockchainStore.Search
{
    public abstract class EventIndexer<TEvent, TSearchDocument> : IndexerBase<EventLog<TEvent>, TSearchDocument>, IIndexer<EventLog<TEvent>> where TEvent : class where TSearchDocument : class
    {
        protected EventIndexer(Func<EventLog<TEvent>, TSearchDocument> mapper, int logsPerIndexBatch) : base(mapper, logsPerIndexBatch) { }
    }

    public abstract class EventIndexer<TSearchDocument> : IndexerBase<FilterLog, TSearchDocument>, IIndexer<FilterLog> where TSearchDocument : class
    {
        protected EventIndexer(Func<FilterLog, TSearchDocument> mapper, int _logsPerIndexBatch) : base(mapper, _logsPerIndexBatch) { }
    }

}
