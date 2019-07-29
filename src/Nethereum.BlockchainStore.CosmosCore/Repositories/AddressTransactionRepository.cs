using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.BlockchainStore.CosmosCore.Entities;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.CosmosCore.Repositories
{
    public class AddressTransactionRepository : CosmosRepositoryBase, IAddressTransactionRepository
    {
        public AddressTransactionRepository(DocumentClient client, string databaseName) : base(client, databaseName, CosmosCollectionName.AddressTransactions)
        {
        }

        public async Task<IAddressTransactionView> FindAsync(string address, HexBigInteger blockNumber, string transactionHash)
        {
            var uri = CreateDocumentUri(
                new CosmosAddressTransaction()
                {
                    Address = address, Hash = transactionHash, BlockNumber = blockNumber.Value.ToString()
                });
            try
            {
                var response = await Client.ReadDocumentAsync<CosmosAddressTransaction>(uri);
                return response.Document;
            }
            catch (DocumentClientException dEx)
            {
                if (dEx.IsNotFound())
                    return null;

                throw;
            }
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO, string address, string error = null, string newContractAddress = null)
        {
            var tx = transactionReceiptVO.MapToStorageEntityForUpsert<CosmosAddressTransaction>(address);
            await UpsertDocumentAsync(tx);
        }
    }
}
