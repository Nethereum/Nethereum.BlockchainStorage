using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.RepositorySearching
{
    public interface ISearchService
    {
        Task<SearchResult> FindFirstAsync(string query);
    }
}