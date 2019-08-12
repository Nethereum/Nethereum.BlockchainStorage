using Nest;
using Nethereum.BlockchainStore.Search.ElasticSearch;
using Nethereum.RPC.Eth.DTOs;
using System;

namespace Nethereum.BlockchainStore.Search.Elastic
{
    public class ElasticTransactionReceiptVOIndexer<TSearchDocument> :
        ElasticIndexerBase<TransactionReceiptVO, TSearchDocument>,
        IIndexer<TransactionReceiptVO> where TSearchDocument : class, IHasId
    {

        public ElasticTransactionReceiptVOIndexer(
            string indexName,
            IElasticClient indexClient,
            Func<TransactionReceiptVO, TSearchDocument> mapper,
            int documentsPerIndexBatch = 1
            )
            : base(indexName, indexClient, (tx) => mapper(tx), documentsPerIndexBatch)
        {

        }
    }
}
