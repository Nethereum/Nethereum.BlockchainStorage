using Microsoft.Azure.Documents.Client;
using Nethereum.BlockchainStore.Repositories;
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

        public async Task UpsertAsync(Transaction transaction, TransactionReceipt transactionReceipt, bool failedCreatingContract, HexBigInteger blockTimestamp, string address, string error = null, bool hasVmStack = false, string newContractAddress = null)
        {
            return;
        }
    }
}
