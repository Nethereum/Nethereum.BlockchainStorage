using System;
using System.Threading.Tasks;
using Microsoft.Azure.Search.Models;
using Nethereum.Contracts;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureSearchService : IDisposable
    {
        Task<IAzureEventIndexer<TEvent>> CreateEventIndexer<TEvent>
            (EventIndexDefinition<TEvent> searchIndexDefinition) 
            where TEvent : class;

        Task<IAzureEventIndexer<TEvent>> CreateEventIndexer<TEvent>
            (string indexName = null, bool addPresetEventLogFields = true) 
            where TEvent : class;

        Task<IAzureEventIndexer<TEvent>> CreateEventIndexer<TEvent, TSearchDocument>
            (Index index, IEventToSearchDocumentMapper<TEvent, TSearchDocument> mapper) 
            where TEvent : class where TSearchDocument : class, new();

        Task<IAzureEventIndexer<TEvent>> CreateEventIndexer<TEvent, TSearchDocument>
            (Index index, Func<EventLog<TEvent>, TSearchDocument> mappingFunc) 
            where TEvent : class where TSearchDocument : class, new();

        Task<IAzureFunctionIndexer<TFunctionMessage>> CreateFunctionIndexer<TFunctionMessage>(
            FunctionIndexDefinition<TFunctionMessage> searchIndexDefinition)
            where TFunctionMessage : FunctionMessage, new();

        Task<IAzureFunctionIndexer<TFunctionMessage>> CreateFunctionIndexer<TFunctionMessage>(
            string indexName = null, bool addPresetEventLogFields = true)
            where TFunctionMessage : FunctionMessage, new();

        Task<IAzureFunctionIndexer<TFunctionMessage>> CreateFunctionIndexer<TFunctionMessage, TSearchDocument>(
            Index index, IFunctionMessageToSearchDocumentMapper<TFunctionMessage, TSearchDocument> mapper)
            where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class, new();

        Task<IAzureFunctionIndexer<TFunctionMessage>> CreateFunctionIndexer<TFunctionMessage, TSearchDocument>(
            Index index, Func<FunctionCall<TFunctionMessage>, TSearchDocument> mapperFunc)
            where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class, new();


        Task DeleteIndexAsync(IndexDefinition searchIndex);
        Task DeleteIndexAsync(string indexName);
        Task<long> CountDocumentsAsync(string indexName);
    }
}