using MongoDB.Driver;
using Nethereum.BlockchainStore.MongoDb.Entities;
using Nethereum.BlockchainStore.MongoDb.Repositories;

namespace Nethereum.BlockchainStore.MongoDb.IndexBuilders
{
    public class BlockIndexBuilder : BaseIndexBuilder<MongoDbBlock>
    {
        public BlockIndexBuilder(IMongoDatabase db) : base(db, MongoDbCollectionName.Blocks)
        {
        }

        public override void EnsureIndexes()
        {
            Compound(true, f => f.BlockNumber, f => f.Hash);
        }
    }
}