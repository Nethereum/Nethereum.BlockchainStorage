using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureTransactionReceiptVOIndexer :
        AzureIndexerBase<TransactionReceiptVO, Dictionary<string, object>>,
        IIndexer<TransactionReceiptVO>
    {

        public AzureTransactionReceiptVOIndexer(
            Index index,
            ISearchIndexClient indexClient,
            Func<TransactionReceiptVO, Dictionary<string, object>> mapper,
            int logsPerIndexBatch = 1
            )
            : base(index, indexClient, (tx) => mapper(tx), logsPerIndexBatch)
        {

        }
    }
}