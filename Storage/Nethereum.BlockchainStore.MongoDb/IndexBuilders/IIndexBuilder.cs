namespace Nethereum.BlockchainStore.MongoDb.IndexBuilders
{
    public interface IIndexBuilder
    {
        void EnsureIndexes();
    }
}