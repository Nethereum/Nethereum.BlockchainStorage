using Microsoft.Azure.Documents.Client;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Nethereum.BlockchainStore.CosmosCore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;

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

        public async Task UpsertAsync(RPC.Eth.DTOs.Transaction transaction, TransactionReceipt transactionReceipt, bool failedCreatingContract, HexBigInteger blockTimestamp, string address, string error = null, bool hasVmStack = false, string newContractAddress = null)
        {
            var tx = new CosmosAddressTransaction();
            tx.Map(transaction, address);
            tx.UpdateRowDates();
            await UpsertDocumentAsync(tx);
        }
    }
}
