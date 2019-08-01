using System;
using System.Threading.Tasks;
using Nethereum.Contracts;

namespace Nethereum.BlockchainStore.Search
{
    public interface ISearchService : IDisposable
    {

        //Task<IFunctionIndexer<TFunctionMessage>> CreateFunctionIndexer<TFunctionMessage>(
        //    FunctionIndexDefinition<TFunctionMessage> searchIndexDefinition)
        //    where TFunctionMessage : FunctionMessage, new();

        //Task<IFunctionIndexer<TFunctionMessage>> CreateFunctionIndexer<TFunctionMessage>(
        //    string indexName = null, bool addPresetEventLogFields = true)
        //    where TFunctionMessage : FunctionMessage, new();

        Task DeleteIndexAsync(string indexName);
        Task<long> CountDocumentsAsync(string indexName);
    }
}