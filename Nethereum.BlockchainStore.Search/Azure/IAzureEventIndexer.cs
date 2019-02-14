using System;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureEventIndexer<TEvent>: 
        IDisposable, 
        IEventIndexer<TEvent>, 
        IAzureIndex where TEvent : class
    {
    }

}