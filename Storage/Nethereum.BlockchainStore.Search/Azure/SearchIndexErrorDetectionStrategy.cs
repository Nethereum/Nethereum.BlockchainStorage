using System;
using Microsoft.Azure.Search;
using Microsoft.Rest.TransientFaultHandling;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class SearchIndexErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        public bool IsTransient(Exception ex)
        {
            return ex is IndexBatchException;
        }
    }
}