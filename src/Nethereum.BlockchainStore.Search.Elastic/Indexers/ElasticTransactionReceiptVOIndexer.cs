using Nest;
using Nethereum.BlockchainStore.Search.ElasticSearch;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Search.Elastic
{
    public class ElasticTransactionReceiptVOIndexer :
        ElasticIndexerBase<TransactionReceiptVO, GenericSearchDocument>,
        IIndexer<TransactionReceiptVO>
    {

        public ElasticTransactionReceiptVOIndexer(
            string indexName,
            IElasticClient indexClient,
            TransactionReceiptVOIndexDefinition indexDefinition,
            int documentsPerIndexBatch = 1
            )
            : base(indexName, indexClient, (tx) => tx.ToGenericElasticSearchDoc(indexDefinition), documentsPerIndexBatch)
        {

        }
    }
}
