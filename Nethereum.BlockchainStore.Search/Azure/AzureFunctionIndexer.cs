using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Nethereum.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureFunctionIndexer<TFunctionMessage, TSearchDocument> : AzureIndexBase,
        IAzureFunctionIndexer<TFunctionMessage> where TFunctionMessage : FunctionMessage, new()
        where TSearchDocument : class, new()
    {
        private readonly IFunctionMessageToSearchDocumentMapper<TFunctionMessage, TSearchDocument> _mapper;

        public AzureFunctionIndexer(Index index, ISearchIndexClient indexClient, IFunctionMessageToSearchDocumentMapper<TFunctionMessage, TSearchDocument> mapper) : base(index, indexClient)
        {
            _mapper = mapper;
        }

        public Task IndexAsync(FunctionCall<TFunctionMessage> transaction) => IndexAsync(new[]{transaction});

        public async Task IndexAsync(IEnumerable<FunctionCall<TFunctionMessage>> transactions)
        {
            var documents = transactions.Select(transaction => _mapper.Map(transaction)).ToArray();
            await BatchUpdateAsync(documents);
            Indexed += documents.Length;
        }
    }

    public class AzureFunctionIndexer<TFunctionMessage> : 
        AzureIndexBase, 
        IAzureFunctionIndexer<TFunctionMessage> where TFunctionMessage : FunctionMessage, new()
    {
        private readonly FunctionIndexDefinition<TFunctionMessage> _searchIndexDefinition;

        public AzureFunctionIndexer(FunctionIndexDefinition<TFunctionMessage> searchIndexDefinition, Index index, ISearchIndexClient indexClient)
            :base(index, indexClient)
        {
            _searchIndexDefinition = searchIndexDefinition;
        }

        public Task IndexAsync(FunctionCall<TFunctionMessage> transaction) => IndexAsync(new[]{transaction});

        public async Task IndexAsync(IEnumerable<FunctionCall<TFunctionMessage>> transactions)
        {
            var documents = transactions.Select(l => l.ToAzureDocument(_searchIndexDefinition)).ToArray();
            await BatchUpdateAsync(documents);
            Indexed += documents.Length;
        }
    }
}