using Microsoft.Azure.Search;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureFunctionIndexer<TFunctionMessage, TSearchDocument> : AzureIndexerBase<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument>,
        IIndexer<TransactionForFunctionVO<TFunctionMessage>> where TFunctionMessage : FunctionMessage, new()
        where TSearchDocument : class, IHasId
    {

        public AzureFunctionIndexer(
            ISearchIndexClient indexClient, 
            Func<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument> mapper,
            int documentsPerIndexBatch = 1) : base(indexClient, mapper, documentsPerIndexBatch)
        {
        }
    }

    public class AzureFunctionIndexer<TFunctionMessage> : 
        AzureIndexerBase<TransactionForFunctionVO<TFunctionMessage>, GenericSearchDocument>,
        IIndexer<TransactionForFunctionVO<TFunctionMessage>> where TFunctionMessage : FunctionMessage, new()
    {

        public AzureFunctionIndexer(
            ISearchIndexClient indexClient,
            FunctionIndexDefinition<TFunctionMessage> searchIndexDefinition,
            int documentsPerIndexBatch = 1
            )
            :base(indexClient, (f) => f.ToAzureDocument(searchIndexDefinition), documentsPerIndexBatch)
        {

        }
    }
}