using Nethereum.Contracts;

namespace Nethereum.BlockchainStore.Search
{
    public interface IEventToSearchDocumentMapper<TFrom, TSearchDocument> 
        where TFrom: class where TSearchDocument : class
    {
        TSearchDocument Map(EventLog<TFrom> from);
    }
}