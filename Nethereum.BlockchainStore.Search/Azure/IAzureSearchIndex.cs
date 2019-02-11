using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureSearchIndex
    {
        Task<DocumentSearchResult<Dictionary<string, object>>> SearchAsync(string text, IList<string> facets = null);
        Task<DocumentSuggestResult<Dictionary<string, object>>> SuggestAsync(string searchText, bool fuzzy = true);
        Task<long> DocumentCountAsync();
    }

}