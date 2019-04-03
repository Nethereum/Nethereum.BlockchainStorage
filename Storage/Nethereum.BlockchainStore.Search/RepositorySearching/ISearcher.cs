using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.RepositorySearching
{
    public interface ISearcher
    {
        Task<SearchResult> FindFirstAsync(string query);
    }
}
