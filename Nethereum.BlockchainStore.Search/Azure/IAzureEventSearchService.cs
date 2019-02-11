using System;
using System.Threading.Tasks;
using Nethereum.Contracts;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureSearchService : IDisposable
    {
        Task DeleteIndexAsync(IndexDefinition searchIndex);
        Task DeleteIndexAsync(string indexName);
    }

    public interface IAzureEventSearchService: IAzureSearchService
    {
        Task<IAzureEventSearchIndex<TEvent>> GetOrCreateIndex<TEvent>(EventIndexDefinition<TEvent> searchIndexDefinition) where TEvent : class;
        Task<IAzureEventSearchIndex<TEvent>> GetOrCreateEventIndex<TEvent>(string indexName = null) where TEvent : class;
    }

    public interface IAzureTransactionSearchService: IAzureSearchService
    {
        Task<IAzureFunctionSearchIndex<TFunctionMessage>> GetOrCreateIndex<TFunctionMessage>(
            FunctionIndexDefinition<TFunctionMessage> searchIndexDefinition)
            where TFunctionMessage : FunctionMessage, new();

        Task<IAzureFunctionSearchIndex<TFunctionMessage>> GetOrCreateFunctionIndex<TFunctionMessage>(
            string indexName = null)
            where TFunctionMessage : FunctionMessage, new();
    }
}