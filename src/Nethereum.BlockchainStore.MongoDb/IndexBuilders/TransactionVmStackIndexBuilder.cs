using MongoDB.Driver;
using Nethereum.BlockchainStore.MongoDb.Entities;
using Nethereum.BlockchainStore.MongoDb.Repositories;

namespace Nethereum.BlockchainStore.MongoDb.IndexBuilders
{
    public class TransactionVmStackIndexBuilder : BaseIndexBuilder<MongoDbTransactionVmStack>
    {
        public TransactionVmStackIndexBuilder(IMongoDatabase db) : base(db, MongoDbCollectionName.TransactionVMStacks)
        {
        }

        public override void EnsureIndexes()
        {
            Index(f => f.Address);
            Index(f => f.TransactionHash);
        }
    }
}