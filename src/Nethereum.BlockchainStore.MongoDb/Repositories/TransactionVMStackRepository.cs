using System.Threading.Tasks;
using MongoDB.Driver;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.MongoDb.Entities;
using Nethereum.BlockchainStore.Repositories;
using Newtonsoft.Json.Linq;

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
            var transactionVmStack = new MongoDbTransactionVmStack();
            transactionVmStack.Map(transactionHash, address, stackTrace);
            transactionVmStack.UpdateRowDates();
            await UpsertDocumentAsync(transactionVmStack);
        }
    }
}