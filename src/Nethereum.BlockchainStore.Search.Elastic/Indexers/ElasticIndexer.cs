using Nest;
using Nethereum.BlockchainStore.Search.ElasticSearch;
using System;

namespace Nethereum.BlockchainStore.Search.Elastic
{
    public class ElasticIndexer<TSource, TSearchDocument>
    : ElasticIndexerBase<TSource, TSearchDocument>, IIndexer<TSource>
        where TSource : class where TSearchDocument : class, IHasId
    {
        public ElasticIndexer(
            string indexName,
            IElasticClient indexClient,
            Func<TSource, TSearchDocument> mapper,
            int documentsPerIndexBatch = 1) : base(
                indexName,
                indexClient,
                mapper,
                documentsPerIndexBatch)
        {

        }
    }
}
