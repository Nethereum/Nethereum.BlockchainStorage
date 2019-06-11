using MongoDB.Driver;
using Nethereum.BlockchainStore.MongoDb.Entities;
using Nethereum.BlockchainStore.MongoDb.Repositories;

namespace Nethereum.BlockchainStore.MongoDb.IndexBuilders
{
    public class TransactionIndexBuilder : BaseIndexBuilder<MongoDbTransaction>
    {
        public TransactionIndexBuilder(IMongoDatabase db) : base(db, MongoDbCollectionName.Transactions)
        {
        }

        public override void EnsureIndexes()
        {
            Index(f => f.Hash);
            Index(f => f.AddressFrom);
            Index(f => f.AddressTo);
            Index(f => f.NewContractAddress);
        }
    }
}