using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Nethereum.RPC.Eth.DTOs;
using System;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureTransactionReceiptVOIndexer<TSearchDocument> :
        AzureIndexerBase<TransactionReceiptVO, TSearchDocument>,
        IIndexer<TransactionReceiptVO> where TSearchDocument : class
    {

        public AzureTransactionReceiptVOIndexer(
            Index index,
            ISearchIndexClient indexClient,
            Func<TransactionReceiptVO, TSearchDocument> mapper,
            int documentsPerIndexBatch = 1
            )
            : base(index, indexClient, (tx) => mapper(tx), documentsPerIndexBatch)
        {

        }
    }
}