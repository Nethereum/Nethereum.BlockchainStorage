using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureIndexer<TSource, TSearchDocument>
    : AzureIndexerBase<TSource, TSearchDocument>, IIndexer<TSource>
        where TSource : class where TSearchDocument : class
    {
        public AzureIndexer(
            ISearchIndexClient indexClient,
            Func<TSource, TSearchDocument> mapper,
            int documentsPerIndexBatch = 1) : base(
                indexClient,
                mapper,
                documentsPerIndexBatch)
        {

        }
    }
}