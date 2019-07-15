using System.Threading.Tasks;
using MongoDB.Driver;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.MongoDb.Entities;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.MongoDb.Repositories
{
    public class AddressTransactionRepository : MongoDbRepositoryBase<MongoDbAddressTransaction>, IAddressTransactionRepository
    {
        public AddressTransactionRepository(IMongoClient client, string databaseName) : base(client, databaseName, MongoDbCollectionName.AddressTransactions)
        {
        }

        public async Task<IAddressTransactionView> FindAsync(string address, HexBigInteger blockNumber, string transactionHash)
        {
            var filter = CreateDocumentFilter(
                new MongoDbAddressTransaction()
                {
                    Address = address, Hash = transactionHash, BlockNumber = blockNumber.Value.ToString()
                });

            var response = await Collection.Find(filter).SingleOrDefaultAsync();
            return response;
        }

        public async Task UpsertAsync(RPC.Eth.DTOs.Transaction transaction, TransactionReceipt transactionReceipt, bool failedCreatingContract, HexBigInteger blockTimestamp, string address, string error = null, bool hasVmStack = false, string newContractAddress = null)
        {
            var tx = new MongoDbAddressTransaction();
            tx.Map(transaction, address);
            tx.UpdateRowDates();
            await UpsertDocumentAsync(tx);
        }
    }
}