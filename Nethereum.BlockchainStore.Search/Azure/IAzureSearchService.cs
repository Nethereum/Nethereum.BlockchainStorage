using System;
using System.Threading.Tasks;
using Microsoft.Azure.Search.Models;
using Nethereum.Contracts;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureSearchService : ISearchService
    {
        Task<IEventIndexer<TEvent>> CreateEventIndexer<TEvent, TSearchDocument>
            (Index index, IEventToSearchDocumentMapper<TEvent, TSearchDocument> mapper) 
            where TEvent : class where TSearchDocument : class, new();

        Task<IEventIndexer<TEvent>> CreateEventIndexer<TEvent, TSearchDocument>
            (Index index, Func<EventLog<TEvent>, TSearchDocument> mappingFunc) 
            where TEvent : class where TSearchDocument : class, new();

        Task<IFunctionIndexer<TFunctionMessage>> CreateFunctionIndexer<TFunctionMessage, TSearchDocument>(
            Index index, IFunctionMessageToSearchDocumentMapper<TFunctionMessage, TSearchDocument> mapper)
            where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class, new();

        Task<IFunctionIndexer<TFunctionMessage>> CreateFunctionIndexer<TFunctionMessage, TSearchDocument>(
            Index index, Func<FunctionCall<TFunctionMessage>, TSearchDocument> mapperFunc)
            where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class, new();
    }
}