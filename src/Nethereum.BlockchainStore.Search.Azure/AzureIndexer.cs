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
            Index index,
            ISearchIndexClient indexClient,
            Func<TSource, TSearchDocument> mapper,
            int logsPerIndexBatch = 1) : base(
                index,
                indexClient,
                mapper,
                logsPerIndexBatch)
        {

        }
    }
}