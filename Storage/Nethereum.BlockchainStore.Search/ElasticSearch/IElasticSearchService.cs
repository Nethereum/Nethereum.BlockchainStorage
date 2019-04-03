using Nethereum.Contracts;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.ElasticSearch
{
    public interface IElasticSearchService : ISearchService
    {
        Task<IEventIndexer<TEvent>> CreateEventIndexer<TEvent, TSearchDocument>
            (string indexName, IEventToSearchDocumentMapper<TEvent, TSearchDocument> mapper) 
            where TEvent : class where TSearchDocument : class, IHasId, new();

        Task<IEventIndexer<TEvent>> CreateEventIndexer<TEvent, TSearchDocument>
            (string indexName, Func<EventLog<TEvent>, TSearchDocument> mappingFunc) 
            where TEvent : class where TSearchDocument : class, IHasId, new();

        Task<IFunctionIndexer<TFunctionMessage>> CreateFunctionIndexer<TFunctionMessage, TSearchDocument>(
            string indexName, IFunctionMessageToSearchDocumentMapper<TFunctionMessage, TSearchDocument> mapper)
            where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class, IHasId, new();

        Task<IFunctionIndexer<TFunctionMessage>> CreateFunctionIndexer<TFunctionMessage, TSearchDocument>(
            string indexName, Func<FunctionCall<TFunctionMessage>, TSearchDocument> mapperFunc)
            where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class, IHasId, new();
    }
}