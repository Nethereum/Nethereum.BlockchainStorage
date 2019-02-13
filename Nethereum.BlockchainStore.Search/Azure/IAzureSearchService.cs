using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureSearchService : IDisposable
    {
        Task DeleteIndexAsync(IndexDefinition searchIndex);
        Task DeleteIndexAsync(string indexName);
    }
}