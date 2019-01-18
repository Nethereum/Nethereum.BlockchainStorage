using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{
    public interface ISearcher
    {
        Task<SearchResult> FindFirstAsync(string query);
    }
}
