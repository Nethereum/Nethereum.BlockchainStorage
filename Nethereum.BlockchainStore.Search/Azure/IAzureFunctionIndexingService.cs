using System.Threading.Tasks;
using Nethereum.Contracts;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureFunctionIndexingService: IAzureSearchService
    {
        Task<IAzureFunctionIndexer<TFunctionMessage>> GetOrCreateIndex<TFunctionMessage>(
            FunctionIndexDefinition<TFunctionMessage> searchIndexDefinition)
            where TFunctionMessage : FunctionMessage, new();

        Task<IAzureFunctionIndexer<TFunctionMessage>> GetOrCreateFunctionIndexer<TFunctionMessage>(
            string indexName = null, bool addPresetEventLogFields = true)
            where TFunctionMessage : FunctionMessage, new();
    }
}