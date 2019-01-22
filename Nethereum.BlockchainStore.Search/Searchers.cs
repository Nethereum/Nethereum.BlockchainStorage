using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{

    public class UnknownSearchTypeSearcher : ISearcher
    {
        public static readonly UnknownSearchTypeSearcher Instance = new UnknownSearchTypeSearcher();

        public Task<SearchResult> FindFirstAsync(string query)
        {
            return Task.FromResult(new SearchResult() {Title = $"Invalid or unsupported query. ({query})"});
        }
    }
}
