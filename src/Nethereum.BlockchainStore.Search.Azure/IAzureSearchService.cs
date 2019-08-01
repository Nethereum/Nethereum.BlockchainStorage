using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureSearchService : ISearchService
    {
        Task<bool> IndexExistsAsync(string indexName);
        Task<Index> CreateIndexAsync(Index index);
        Task<Index> GetIndexAsync(string indexName);

        ISearchIndexClient GetOrCreateIndexClient(string indexName);

        //FilterLog - default mapping
        //Task<IIndexer<FilterLog>> CreateEventIndexer
        //    (Index index);

        ////FilterLog - custom mapping
        //Task<IIndexer<FilterLog>> CreateEventIndexer<TSearchDocument>
        //    (Index index, Func<FilterLog, TSearchDocument> mapper) where TSearchDocument : class;

        ////EventLog<T> with implicit mapping
        //Task<IIndexer<EventLog<TEvent>>> CreateEventIndexer<TEvent>
        //    (EventIndexDefinition<TEvent> searchIndexDefinition)
        //        where TEvent : class;

        ////EventLog<T> with custom mapping to TSearchDocument
        //Task<IIndexer<EventLog<TEvent>>> CreateEventIndexer<TEvent, TSearchDocument>
        //    (Index index, Func<EventLog<TEvent>, TSearchDocument> mapper)
        //        where TEvent : class where TSearchDocument : class;


        //Task<IEventIndexer<TEvent>> CreateEventIndexer<TEvent, TSearchDocument>
        //    (Index index, Func<EventLog<TEvent>, TSearchDocument> mappingFunc) 
        //    where TEvent : class where TSearchDocument : class, new();

        //Task<IFunctionIndexer<TFunctionMessage>> CreateFunctionIndexer<TFunctionMessage, TSearchDocument>(
        //    Index index, Func<FunctionCall<TFunctionMessage>, TSearchDocument> mapperFunc)
        //    where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class, new();
    }
}