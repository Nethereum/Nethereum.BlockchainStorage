using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Nethereum.Contracts;

namespace Nethereum.BlockchainStore.Search
{
    public interface IEventSearchIndexer<TEvent> where TEvent : class
    {
        Task IndexAsync(EventLog<TEvent> log) ;
        Task IndexAsync(IEnumerable<EventLog<TEvent>> logs) ;
        int Indexed { get; }
    }
}
