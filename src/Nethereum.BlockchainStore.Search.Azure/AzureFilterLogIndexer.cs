using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;

namespace Nethereum.BlockchainStore.Search.Azure
{
    // filter log with mapping
    public class AzureFilterLogIndexer<TSearchDocument>
    : AzureIndexerBase<FilterLog, TSearchDocument>, IIndexer<FilterLog>
        where TSearchDocument : class
    {
        public AzureFilterLogIndexer(
            Index index,
            ISearchIndexClient indexClient,
            Func<FilterLog, TSearchDocument> mapper,
            int logsPerIndexBatch = 1) : base(
                index,
                indexClient,
                mapper,
                logsPerIndexBatch)
        {

        }
    }

    public class AzureFilterLogIndexer
    : AzureIndexerBase<FilterLog, Dictionary<string, object>>, IIndexer<FilterLog>
    {
        public AzureFilterLogIndexer(
            Index index,
            ISearchIndexClient indexClient,
            int logsPerIndexBatch = 1) : base(
                index,
                indexClient,
                (filterLog) => FilterLogSearchIndex.Map(filterLog),
                logsPerIndexBatch)
        {

        }
    }
}