using System.Threading.Tasks;
using MongoDB.Driver;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.MongoDb.Entities;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.MongoDb.Repositories
{
    public class TransactionLogRepository : MongoDbRepositoryBase<MongoDbTransactionLog>, ITransactionLogRepository
    {
        public TransactionLogRepository(IMongoClient client, string databaseName) : base(client, databaseName, MongoDbCollectionName.TransactionLogs)
        {
        }

        public async Task<ITransactionLogView> FindByTransactionHashAndLogIndexAsync(string hash, long idx)
        {
            var filter = CreateDocumentFilter(new MongoDbTransactionLog {TransactionHash = hash, LogIndex = idx});

            var response = await Collection.Find(filter).SingleOrDefaultAsync();
            return response;
        }

        public async Task UpsertAsync(FilterLog log)
        {
            var transactionLog = new MongoDbTransactionLog();
            transactionLog.Map(log);
            transactionLog.UpdateRowDates();
            await UpsertDocumentAsync(transactionLog);
        }
    }
}