using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Nethereum.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureFunctionSearchSearchIndex<TFunctionMessage> : 
        AzureSearchIndexBase, 
        IAzureFunctionSearchIndex<TFunctionMessage> where TFunctionMessage : FunctionMessage, new()
    {
        private readonly FunctionIndexDefinition<TFunctionMessage> _searchIndexDefinition;

        public AzureFunctionSearchSearchIndex(FunctionIndexDefinition<TFunctionMessage> searchIndexDefinition, Index index, ISearchIndexClient indexClient)
            :base(index, indexClient)
        {
            _searchIndexDefinition = searchIndexDefinition;
        }

        public Task IndexAsync(Transaction tx, TFunctionMessage functionMessage) => IndexAsync(new[]{(tx, functionMessage)});

        public async Task IndexAsync(IEnumerable<(Transaction tx, TFunctionMessage functionMessage)> transactions)
        {
            var documents = transactions.Select(l => l.ToAzureDocument(_searchIndexDefinition)).ToArray();
            await BatchUpdateAsync(documents);
            Indexed += documents.Length;
        }
    }
}