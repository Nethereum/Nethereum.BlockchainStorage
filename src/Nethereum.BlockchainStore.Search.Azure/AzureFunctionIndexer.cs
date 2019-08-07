using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureFunctionIndexer<TFunctionMessage, TSearchDocument> : AzureIndexerBase<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument>,
        IIndexer<TransactionForFunctionVO<TFunctionMessage>> where TFunctionMessage : FunctionMessage, new()
        where TSearchDocument : class
    {

        public AzureFunctionIndexer(
            Index index, 
            ISearchIndexClient indexClient, 
            Func<TransactionForFunctionVO<TFunctionMessage>, TSearchDocument> mapper,
            int logsPerIndexBatch = 1) : base(index, indexClient, mapper, logsPerIndexBatch)
        {
        }
    }

    public class AzureFunctionIndexer<TFunctionMessage> : 
        AzureIndexerBase<TransactionForFunctionVO<TFunctionMessage>, Dictionary<string, object>>,
        IIndexer<TransactionForFunctionVO<TFunctionMessage>> where TFunctionMessage : FunctionMessage, new()
    {

        public AzureFunctionIndexer(
            Index index,
            ISearchIndexClient indexClient,
            FunctionIndexDefinition<TFunctionMessage> searchIndexDefinition,
            int logsPerIndexBatch = 1
            )
            :base(index, indexClient, (f) => f.ToAzureDocument(searchIndexDefinition), logsPerIndexBatch)
        {

        }
    }
}