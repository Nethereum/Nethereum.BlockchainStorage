using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Nethereum.BlockchainStore.Search.Services;
using Nethereum.Contracts;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureSearchService: ISearchService
    {
        Task<Index> GetIndexAsync(string indexName);
        Task<Index> CreateIndexAsync(Index index);
        Task<Index> CreateIndexAsync(IndexDefinition indexDefinition);
        Task<Index> CreateIndexForEventLogAsync<TEventDTO>(string indexName = null) where TEventDTO : class;
        Task<Index> CreateIndexForLogAsync(string indexName);
        Task<Index> CreateIndexForFunctionMessageAsync<TFunctionMessage>(string indexName = null)
            where TFunctionMessage : FunctionMessage;

        ISearchIndexClient GetOrCreateIndexClient(string indexName);

        IAzureIndexSearcher CreateIndexSearcher(Index index);

    }
}