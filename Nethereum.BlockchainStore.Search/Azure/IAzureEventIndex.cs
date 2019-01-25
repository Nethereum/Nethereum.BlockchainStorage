using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Search.Models;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureEventIndex<TEvent>: IDisposable where TEvent : class, IEventDTO, new()
    {
        Task<DocumentSearchResult<Dictionary<string, object>>> SearchAsync(string text, IList<string> facets = null);
        Task<DocumentSuggestResult<Dictionary<string, object>>> SuggestAsync(string searchText, bool fuzzy = true);
        Task UpsertAsync(EventLog<TEvent> log);
    }
}