using MongoDB.Driver;
using Nethereum.BlockchainProcessing.Storage.Entities;
using Nethereum.BlockchainProcessing.Storage.Entities.Mapping;
using Nethereum.BlockchainProcessing.Storage.Repositories;
using Nethereum.BlockchainStore.MongoDb.Entities;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

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

            var response = await Collection.Find(filter).SingleOrDefaultAsync().ConfigureAwait(false);
            return response;
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO, string address, string error = null, string newContractAddress = null)
        {
            var tx = transactionReceiptVO.MapToStorageEntityForUpsert<MongoDbAddressTransaction>(address);
            tx.UpdateRowDates();
            await UpsertDocumentAsync(tx).ConfigureAwait(false);
        }
    }
}