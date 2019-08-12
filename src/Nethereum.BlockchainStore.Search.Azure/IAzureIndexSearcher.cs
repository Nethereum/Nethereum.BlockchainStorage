using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureIndexSearcher : IDisposable
    {
        Index Index { get; }
        ISearchIndexClient SearchIndexClient { get; }

        Task<DocumentSearchResult<Dictionary<string, object>>> SearchAsync(string text, IList<string> facets = null);
        Task<DocumentSuggestResult<Dictionary<string, object>>> SuggestAsync(string searchText, bool fuzzy = true);
    }
}
