namespace Nethereum.BlockchainStore.Search.RepositorySearching
{
    public interface ISearcherFactory
    {
        ISearcher[] FindSearchers(string query);
    }
}
