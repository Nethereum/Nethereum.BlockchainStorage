using MongoDB.Driver;
using Nethereum.BlockchainStore.MongoDb.Entities;
using Nethereum.BlockchainStore.MongoDb.Repositories;

namespace Nethereum.BlockchainStore.MongoDb.IndexBuilders
{
    public class ContractIndexBuilder : BaseIndexBuilder<MongoDbContract>
    {
        public ContractIndexBuilder(IMongoDatabase db) : base(db, MongoDbCollectionName.Contracts)
        {
        }

        public override void EnsureIndexes()
        {
            Index(f => f.Name);
            Index(f => f.Address, true);
        }
    }
}