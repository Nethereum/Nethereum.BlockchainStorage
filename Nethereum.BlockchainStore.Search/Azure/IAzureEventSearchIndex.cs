using System;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureEventSearchIndex<TEvent>: 
        IDisposable, 
        IEventIndexer<TEvent>, 
        IAzureSearchIndex where TEvent : class
    {
    }

}