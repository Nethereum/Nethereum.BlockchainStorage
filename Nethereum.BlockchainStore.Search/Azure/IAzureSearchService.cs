using System;
using System.Threading.Tasks;
using Nethereum.Contracts;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureSearchService : IDisposable
    {
        Task<IAzureEventIndexer<TEvent>> GetOrCreateEventIndexer<TEvent>
            (EventIndexDefinition<TEvent> searchIndexDefinition) 
            where TEvent : class;

        Task<IAzureEventIndexer<TEvent>> GetOrCreateEventIndexer<TEvent>
            (string indexName = null, bool addPresetEventLogFields = true) 
            where TEvent : class;

        Task<IAzureFunctionIndexer<TFunctionMessage>> GetOrCreateFunctionIndexer<TFunctionMessage>(
            FunctionIndexDefinition<TFunctionMessage> searchIndexDefinition)
            where TFunctionMessage : FunctionMessage, new();

        Task<IAzureFunctionIndexer<TFunctionMessage>> GetOrCreateFunctionIndexer<TFunctionMessage>(
            string indexName = null, bool addPresetEventLogFields = true)
            where TFunctionMessage : FunctionMessage, new();

        Task DeleteIndexAsync(IndexDefinition searchIndex);
        Task DeleteIndexAsync(string indexName);
        Task<long> CountDocumentsAsync(string indexName);
    }
}