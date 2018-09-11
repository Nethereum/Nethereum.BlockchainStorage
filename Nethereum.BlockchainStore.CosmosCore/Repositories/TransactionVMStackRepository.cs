using System.Net;
using Microsoft.Azure.Documents.Client;
using Nethereum.BlockchainStore.CosmosCore.Entities;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Repositories;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace Nethereum.BlockchainStore.CosmosCore.Repositories
{
    public class TransactionVMStackRepository : CosmosRepositoryBase,  IEntityTransactionVMStackRepository
    {
        public TransactionVMStackRepository(DocumentClient client, string databaseName) : base(client, databaseName, CosmosCollectionName.TransactionVMStacks)
        {
        }

        public async Task<TransactionVmStack> FindByTransactionHashAync(string hash)
        {
            var uri = CreateDocumentUri(new CosmosTransactionVmStack(){TransactionHash = hash});
            try
            {
                var response = await Client.ReadDocumentAsync<CosmosTransactionVmStack>(uri);
                return response.Document;
            }
            catch (DocumentClientException dEx)
            {
                if (dEx.IsNotFound())
                    return null;

                throw;
            }
        }

        public async Task Remove(TransactionVmStack transactionVmStack)
        {
            var uri = CreateDocumentUri(transactionVmStack.TransactionHash);
            try
            {
                await Client.DeleteDocumentAsync(uri);
            }
            catch (DocumentClientException dEx)
            {
                if (dEx.IsNotFound())
                    return;

                throw;
            }
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
