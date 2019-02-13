using System.Threading.Tasks;
using Nethereum.Contracts;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureFunctionSearchService: IAzureSearchService
    {
        Task<IAzureFunctionSearchIndex<TFunctionMessage>> GetOrCreateIndex<TFunctionMessage>(
            FunctionIndexDefinition<TFunctionMessage> searchIndexDefinition)
            where TFunctionMessage : FunctionMessage, new();

        Task<IAzureFunctionSearchIndex<TFunctionMessage>> GetOrCreateFunctionIndex<TFunctionMessage>(
            string indexName = null)
            where TFunctionMessage : FunctionMessage, new();
    }
}