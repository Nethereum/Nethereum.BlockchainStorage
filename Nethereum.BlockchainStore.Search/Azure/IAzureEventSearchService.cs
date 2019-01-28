using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureEventSearchService: IDisposable
    {
        Task DeleteIndexAsync(SearchIndexDefinition searchIndex);
        Task DeleteIndexAsync(string indexName);
        Task<IAzureEventSearchIndex<TEvent>> GetOrCreateIndex<TEvent>(EventSearchIndexDefinition<TEvent> searchIndexDefinition) where TEvent : class;
        Task<IAzureEventSearchIndex<TEvent>> GetOrCreateIndex<TEvent>(string indexName = null) where TEvent : class;
    }
}