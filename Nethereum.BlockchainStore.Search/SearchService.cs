using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{

    public class SearchService : ISearchService
    {
        private readonly ISearcherFactory _searcherFactory;

        public SearchService(ISearcherFactory searcherFactory)
        {
            _searcherFactory = searcherFactory;
        }

        public async Task<SearchResult> FindFirstAsync(string query)
        {
            foreach(var searcher in _searcherFactory.FindSearchers(query))
            {
                var result = await searcher.FindFirstAsync(query);
                if (result != null)
                {
                    return result;
                }
            }

            return SearchResult.Empty;
        }
    }




}
