using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureTransactionReceiptVOIndexer :
        AzureIndexerBase<TransactionReceiptVO, Dictionary<string, object>>,
        IIndexer<TransactionReceiptVO>
    {

        public AzureTransactionReceiptVOIndexer(
            ISearchIndexClient indexClient,
            TransactionReceiptVOIndexDefinition indexDefinition,
            int documentsPerIndexBatch = 1
            )
            : base(indexClient, (tx) => tx.ToAzureDocument(indexDefinition), documentsPerIndexBatch)
        {

        }
    }
}