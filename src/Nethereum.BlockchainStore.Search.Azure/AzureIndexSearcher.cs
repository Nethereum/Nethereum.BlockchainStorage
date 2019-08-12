using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureIndexSearcher : IAzureIndexSearcher
    {
        public Index Index { get; }
        public ISearchIndexClient SearchIndexClient { get; }

        public AzureIndexSearcher(Index index, ISearchIndexClient searchIndexClient)
        {
            Index = index;
            SearchIndexClient = searchIndexClient;
        }

        public virtual async Task<DocumentSuggestResult<Dictionary<string, object>>> SuggestAsync(string searchText, bool fuzzy = true)
        {
            var sp = new SuggestParameters
            {
                UseFuzzyMatching = fuzzy,
                Top = 8
            };

            return await SearchIndexClient
                .Documents
                .SuggestAsync<Dictionary<string, object>>(searchText, AzureSearchExtensions.SuggesterName, sp);
        }

        public virtual async Task<DocumentSearchResult<Dictionary<string, object>>> SearchAsync(string text, IList<string> facets = null)
        {
            var sp = new SearchParameters
            {
                SearchMode = SearchMode.All,
                Top = 20,
                Facets = facets ?? Index.FacetableFieldNames(),
                IncludeTotalResultCount = true
            };

            return await SearchIndexClient
                .Documents
                .SearchAsync<Dictionary<string, object>>(text, sp);
        }

        public void Dispose()
        {
            SearchIndexClient.Dispose();
        }
    }
}
