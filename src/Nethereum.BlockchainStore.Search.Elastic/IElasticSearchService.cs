using Nethereum.BlockchainStore.Search.Services;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.ElasticSearch
{
    public interface IElasticSearchService : ISearchService
    {
        Task CreateIfNotExists(IndexDefinition searchIndexDefinition);
        Task CreateIfNotExists(string indexName);
    }
}