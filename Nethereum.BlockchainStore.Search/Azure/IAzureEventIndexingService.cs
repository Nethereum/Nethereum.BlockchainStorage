using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.Azure
{

    public interface IAzureEventIndexingService: IAzureSearchService
    {
        Task<IAzureEventIndexer<TEvent>> GetOrCreateIndex<TEvent>(EventIndexDefinition<TEvent> searchIndexDefinition) where TEvent : class;
        Task<IAzureEventIndexer<TEvent>> GetOrCreateEventIndexer<TEvent>(string indexName = null, bool addPresetEventLogFields = true) where TEvent : class;
    }
}