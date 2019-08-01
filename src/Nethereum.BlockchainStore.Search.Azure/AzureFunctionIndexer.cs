using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureFunctionIndexer<TFunctionMessage, TSearchDocument> : AzureIndexerBase<FunctionCall<TFunctionMessage>, TSearchDocument>,
        IFunctionIndexer<TFunctionMessage> where TFunctionMessage : FunctionMessage, new()
        where TSearchDocument : class, new()
    {

        public AzureFunctionIndexer(
            Index index, 
            ISearchIndexClient indexClient, 
            Func<FunctionCall<TFunctionMessage>, TSearchDocument> mapper,
            int logsPerIndexBatch = 1) : base(index, indexClient, mapper, logsPerIndexBatch)
        {
        }
    }

    public class AzureFunctionIndexer<TFunctionMessage> : 
        AzureIndexerBase<FunctionCall<TFunctionMessage>, Dictionary<string, object>>, 
        IFunctionIndexer<TFunctionMessage> where TFunctionMessage : FunctionMessage, new()
    {

        public AzureFunctionIndexer(
            FunctionIndexDefinition<TFunctionMessage> searchIndexDefinition, 
            Index index, 
            ISearchIndexClient indexClient,
            int logsPerIndexBatch = 1
            )
            :base(index, indexClient, (f) => f.ToAzureDocument(searchIndexDefinition), logsPerIndexBatch)
        {

        }
    }
}