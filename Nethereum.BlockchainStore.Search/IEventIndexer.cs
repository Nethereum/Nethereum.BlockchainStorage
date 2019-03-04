using System;
using Nethereum.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{

    public interface IEventIndexer<TEvent> : IIndexer, IDisposable where TEvent : class
    {
        Task IndexAsync(EventLog<TEvent> log) ;
        Task IndexAsync(IEnumerable<EventLog<TEvent>> logs) ;
    }
}
