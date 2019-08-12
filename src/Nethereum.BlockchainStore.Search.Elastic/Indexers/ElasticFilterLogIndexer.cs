using Nest;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;

namespace Nethereum.BlockchainStore.Search.ElasticSearch
{
    public class ElasticFilterLogIndexer<TSearchDocument>
    : ElasticIndexerBase<FilterLog, TSearchDocument>, IIndexer<FilterLog>
        where TSearchDocument : class, IHasId
    {
        public ElasticFilterLogIndexer(
            string indexName,
            IElasticClient indexClient,
            Func<FilterLog, TSearchDocument> mapper,
            int logsPerIndexBatch = 1) : base(
                indexName,
                indexClient,
                mapper,
                logsPerIndexBatch)
        {

        }
    }

    public class ElasticFilterLogIndexer
    : ElasticIndexerBase<FilterLog, GenericSearchDocument>, IIndexer<FilterLog>
    {
        public ElasticFilterLogIndexer(
            string indexName,
            IElasticClient indexClient,
            int logsPerIndexBatch = 1) : base(
                indexName,
                indexClient,
                (filterLog) => filterLog.ToGenericElasticSearchDoc(PresetSearchFields.LogFields),
                logsPerIndexBatch)
        {

        }
    }
}
