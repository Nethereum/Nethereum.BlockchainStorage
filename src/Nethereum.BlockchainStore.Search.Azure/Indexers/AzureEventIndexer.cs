using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;

namespace Nethereum.BlockchainStore.Search.Azure
{
    // event with mapper
    public class AzureEventIndexer<TEvent, TSearchDocument>
    : AzureIndexerBase<EventLog<TEvent>, TSearchDocument>, IIndexer<EventLog<TEvent>>
        where TEvent : class where TSearchDocument : class
    {
        public AzureEventIndexer(
            ISearchIndexClient indexClient,
            Func<EventLog<TEvent>, TSearchDocument> mapper, 
            int logsPerIndexBatch = 1) : 
                base(indexClient, mapper, logsPerIndexBatch){}
    }

    //event with implicit mapping to default search doc (dictionary<string, object>)
    public class AzureEventIndexer<TEvent>
    : AzureIndexerBase<EventLog<TEvent>, Dictionary<string, object>>, IIndexer<EventLog<TEvent>>
        where TEvent : class
    {
        public AzureEventIndexer(
            ISearchIndexClient indexClient,
            EventIndexDefinition<TEvent> indexDefinition,
            int logsPerIndexBatch = 1) : 
                base(
                    indexClient,
                    (e) => e.ToAzureDocument(indexDefinition),
                    logsPerIndexBatch){}
    }
}