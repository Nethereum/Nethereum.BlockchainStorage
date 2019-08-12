using Nest;
using Nethereum.Contracts;
using System;

namespace Nethereum.BlockchainStore.Search.ElasticSearch
{
    public class ElasticEventIndexer<TEvent, TSearchDocument>
    : ElasticIndexerBase<EventLog<TEvent>, TSearchDocument>, IIndexer<EventLog<TEvent>>
        where TEvent : class where TSearchDocument : class, IHasId
    {
        public ElasticEventIndexer(
            string indexName,
            IElasticClient indexClient,
            Func<EventLog<TEvent>, TSearchDocument> mapper,
            int logsPerIndexBatch = 1) :
                base(indexName, indexClient, mapper, logsPerIndexBatch)
        { }
    }

    //event with implicit mapping to default search doc
    public class ElasticEventIndexer<TEvent>
    : ElasticIndexerBase<EventLog<TEvent>, GenericSearchDocument>, IIndexer<EventLog<TEvent>>
        where TEvent : class
    {
        public ElasticEventIndexer(
            string indexName,
            IElasticClient indexClient,
            EventIndexDefinition<TEvent> indexDefinition,
            int logsPerIndexBatch = 1) :
                base(
                    indexName,
                    indexClient,
                    (e) => e.ToGenericElasticSearchDoc(indexDefinition),
                    logsPerIndexBatch)
        { }
    }
}
