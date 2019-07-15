using MongoDB.Driver;
using Nethereum.BlockchainProcessing.Storage.Entities;
using Nethereum.BlockchainProcessing.Storage.Entities.Mapping;
using Nethereum.BlockchainProcessing.Storage.Repositories;
using Nethereum.BlockchainStore.MongoDb.Entities;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using System.Numerics;

namespace Nethereum.BlockchainStore.MongoDb.Repositories
{
    public class TransactionLogRepository : MongoDbRepositoryBase<MongoDbTransactionLog>, ITransactionLogRepository
    {
        public TransactionLogRepository(IMongoClient client, string databaseName) : base(client, databaseName, MongoDbCollectionName.TransactionLogs)
        {
        }

        public async Task<ITransactionLogView> FindByTransactionHashAndLogIndexAsync(string hash, BigInteger idx)
        {
            var filter = CreateDocumentFilter(new MongoDbTransactionLog {TransactionHash = hash, LogIndex = idx.ToString()});

            var response = await Collection.Find(filter).SingleOrDefaultAsync().ConfigureAwait(false);
            return response;
        }

        public async Task UpsertAsync(FilterLogVO log)
        {
            var transactionLog = log.MapToStorageEntityForUpsert<MongoDbTransactionLog>();
            await UpsertDocumentAsync(transactionLog).ConfigureAwait(false);
        }
    }
}