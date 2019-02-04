using Microsoft.Azure.Search.Models;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureEventSearchIndex<TEvent>: IDisposable, IIndexer<TEvent> where TEvent : class
    {
        Task<DocumentSearchResult<Dictionary<string, object>>> SearchAsync(string text, IList<string> facets = null);
        Task<DocumentSuggestResult<Dictionary<string, object>>> SuggestAsync(string searchText, bool fuzzy = true);
        Task<long> DocumentCountAsync();
    }
}