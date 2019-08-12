using Nest;
using Nethereum.Contracts;
using System;

namespace Nethereum.BlockchainStore.Search.ElasticSearch
{
    public class ElasticFunctionIndexer<TFunctionMessage, TSearchDocument> :
        ElasticIndexerBase<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument>,
        IIndexer<TransactionForFunctionVO<TFunctionMessage>> where TFunctionMessage : FunctionMessage, new()
        where TSearchDocument : class, IHasId
    {

        public ElasticFunctionIndexer(
            string indexName,
            IElasticClient indexClient,
            Func<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument> mapper,
            int documentsPerIndexBatch = 1) : base(indexName, indexClient, mapper, documentsPerIndexBatch)
        {
        }
    }

    public class ElasticFunctionIndexer<TFunctionMessage> :
        ElasticIndexerBase<TransactionForFunctionVO<TFunctionMessage>, GenericSearchDocument>,
        IIndexer<TransactionForFunctionVO<TFunctionMessage>> where TFunctionMessage : FunctionMessage, new()
    {

        public ElasticFunctionIndexer(
            string indexName,
            IElasticClient indexClient,
            FunctionIndexDefinition<TFunctionMessage> searchIndexDefinition,
            int documentsPerIndexBatch = 1
            )
            : base(indexName, indexClient, (f) => f.ToGenericElasticSearchDoc(searchIndexDefinition), documentsPerIndexBatch)
        {

        }
    }

}
