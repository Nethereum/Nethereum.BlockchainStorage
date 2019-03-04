using System;
using System.Threading.Tasks;
using Nethereum.Contracts;

namespace Nethereum.BlockchainStore.Search
{
    public interface ISearchService : IDisposable
    {
        Task<IEventIndexer<TEvent>> CreateEventIndexer<TEvent>
            (EventIndexDefinition<TEvent> searchIndexDefinition) 
            where TEvent : class;

        Task<IEventIndexer<TEvent>> CreateEventIndexer<TEvent>
            (string indexName = null, bool addPresetEventLogFields = true) 
            where TEvent : class;

        Task<IFunctionIndexer<TFunctionMessage>> CreateFunctionIndexer<TFunctionMessage>(
            FunctionIndexDefinition<TFunctionMessage> searchIndexDefinition)
            where TFunctionMessage : FunctionMessage, new();

        Task<IFunctionIndexer<TFunctionMessage>> CreateFunctionIndexer<TFunctionMessage>(
            string indexName = null, bool addPresetEventLogFields = true)
            where TFunctionMessage : FunctionMessage, new();

        Task DeleteIndexAsync(IndexDefinition searchIndex);
        Task DeleteIndexAsync(string indexName);
        Task<long> CountDocumentsAsync(string indexName);
    }
}