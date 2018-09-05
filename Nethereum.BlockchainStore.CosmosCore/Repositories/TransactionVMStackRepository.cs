using Microsoft.Azure.Documents.Client;
using Nethereum.BlockchainStore.CosmosCore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Repositories;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.CosmosCore.Repositories
{
    public class TransactionVMStackRepository : CosmosRepositoryBase,  ITransactionVMStackRepository
    {
        public TransactionVMStackRepository(DocumentClient client, string databaseName) : base(client, databaseName, CosmosCollectionName.TransactionVMStacks)
        {
        }

        public async Task UpsertAsync(string transactionHash, string address, JObject stackTrace)
        {
            var transactionVmStack = new CosmosTransactionVmStack();
            transactionVmStack.Map(transactionHash, address, stackTrace);
            transactionVmStack.UpdateRowDates();
            await UpsertDocumentAsync(transactionVmStack);
        }
    }
}
