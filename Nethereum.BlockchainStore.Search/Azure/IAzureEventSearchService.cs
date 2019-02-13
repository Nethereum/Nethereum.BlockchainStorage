using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.Azure
{

    public interface IAzureEventSearchService: IAzureSearchService
    {
        Task<IAzureEventSearchIndex<TEvent>> GetOrCreateIndex<TEvent>(EventIndexDefinition<TEvent> searchIndexDefinition) where TEvent : class;
        Task<IAzureEventSearchIndex<TEvent>> GetOrCreateEventIndex<TEvent>(string indexName = null) where TEvent : class;
    }
}