using System.Collections.Generic;

namespace Nethereum.BlockchainStore.Search
{
    public interface ISearcherFactory
    {
        ISearcher[] FindSearchers(string query);
    }
}
