using MongoDB.Driver;
using Nethereum.BlockchainStore.MongoDb.Entities;
using Nethereum.BlockchainStore.MongoDb.Repositories;

namespace Nethereum.BlockchainStore.MongoDb.IndexBuilders
{
    public class BlockProgressIndexBuilder : BaseIndexBuilder<MongoDbBlockProgress>
    {
        public BlockProgressIndexBuilder(IMongoDatabase db) : base(db, MongoDbCollectionName.BlockProgress)
        {
        }

        public override void EnsureIndexes()
        {
            Compound(true, f => f.LastBlockProcessed);
        }
    }
}