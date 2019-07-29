using MongoDB.Driver;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.BlockchainStore.MongoDb.Entities;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.MongoDb.Repositories
{
    public class TransactionVMStackRepository : MongoDbRepositoryBase<MongoDbTransactionVmStack>, ITransactionVMStackRepository
    {
        public TransactionVMStackRepository(IMongoClient client, string databaseName) : base(client, databaseName, MongoDbCollectionName.TransactionVMStacks)
        {
        }

        public async Task<ITransactionVmStackView> FindByAddressAndTransactionHashAync(string address, string hash)
        {
            var filter = CreateDocumentFilter(new MongoDbTransactionVmStack()
                {Address = address, TransactionHash = hash});

            var response = await Collection.Find(filter).SingleOrDefaultAsync();
            return response;
        }

        public async Task<ITransactionVmStackView> FindByTransactionHashAync(string hash)
        {
            var filter = CreateDocumentFilter(new MongoDbTransactionVmStack() {TransactionHash = hash});

            var response = await Collection.Find(filter).SingleOrDefaultAsync();
            return response;
        }

        public async Task UpsertAsync(string transactionHash, string address, JObject stackTrace)
        {
            var transactionVmStack = stackTrace.MapToStorageEntityForUpsert<MongoDbTransactionVmStack>(transactionHash, address);
            await UpsertDocumentAsync(transactionVmStack);
        }
    }
}