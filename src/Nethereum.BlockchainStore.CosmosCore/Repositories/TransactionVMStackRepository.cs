using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.BlockchainStore.CosmosCore.Entities;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.CosmosCore.Repositories
{
    public class TransactionVMStackRepository : CosmosRepositoryBase,  ITransactionVMStackRepository
    {
        public TransactionVMStackRepository(DocumentClient client, string databaseName) : base(client, databaseName, CosmosCollectionName.TransactionVMStacks)
        {
        }

        public async Task<ITransactionVmStackView> FindByAddressAndTransactionHashAsync(string address, string hash)
        {
            var uri = CreateDocumentUri(new CosmosTransactionVmStack(){Address = address, TransactionHash = hash});
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

        public async Task<ITransactionVmStackView> FindByTransactionHashAsync(string hash)
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

        public async Task UpsertAsync(string transactionHash, string address, JObject stackTrace)
        {
            var transactionVmStack = stackTrace.MapToStorageEntityForUpsert<CosmosTransactionVmStack>(transactionHash, address);
            await UpsertDocumentAsync(transactionVmStack);
        }
    }
}
