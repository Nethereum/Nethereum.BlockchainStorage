using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{
    public interface ISearchService
    {
        Task<SearchResult> FindFirstAsync(string query);
    }
}