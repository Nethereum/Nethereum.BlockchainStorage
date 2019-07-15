using MongoDB.Driver;
using Nethereum.BlockchainStore.MongoDb.Entities;
using Nethereum.BlockchainStore.MongoDb.Repositories;

namespace Nethereum.BlockchainStore.MongoDb.IndexBuilders
{
    public class AddressTransactionIndexBuilder : BaseIndexBuilder<MongoDbAddressTransaction>
    {
        public AddressTransactionIndexBuilder(IMongoDatabase db) : base(db, MongoDbCollectionName.AddressTransactions)
        {
        }

        public override void EnsureIndexes()
        {
            Compound(true, f => f.BlockNumber, f => f.Hash, f => f.Address);
            Index(f => f.Hash);
            Index(f => f.Address);
        }
    }
}