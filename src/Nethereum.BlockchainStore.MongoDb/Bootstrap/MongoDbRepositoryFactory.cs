using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Nethereum.BlockchainStore.MongoDb.IndexBuilders;
using Nethereum.BlockchainStore.MongoDb.Repositories;
using Nethereum.BlockchainStore.Repositories;

namespace Nethereum.BlockchainStore.MongoDb.Bootstrap
{
    public class MongoDbRepositoryFactory : IBlockchainStoreRepositoryFactory
    {
        public static MongoDbRepositoryFactory Create(IConfigurationRoot config, bool deleteAllExistingCollections = false)
        {
            var connectionString = config.GetMongoDbConnectionStringOrThrow();
            var tag = config.GetMongoDbTag();
            var locale = config.GetMongoDbLocale();

            var factory = new MongoDbRepositoryFactory(connectionString, tag);

            var db = factory.CreateDbIfNotExists();

            if (deleteAllExistingCollections)
                factory.DeleteAllCollections(db).Wait();

            factory.CreateCollectionsIfNotExist(db, locale).Wait();

            return factory;
        }

        private readonly IMongoClient _client;
        private readonly string _databaseName;

        public MongoDbRepositoryFactory(string connectionString, string dbTag)
        {
            _databaseName = "BlockchainStorage" + dbTag ?? string.Empty;
            _client = new MongoClient(connectionString);
        }

        public IMongoDatabase CreateDbIfNotExists()
        {
            return _client.GetDatabase(_databaseName);
        }

        public async Task DeleteDatabase()
        {
            await _client.DropDatabaseAsync(_databaseName);
        }

        public async Task CreateCollectionsIfNotExist(IMongoDatabase db, string locale)
        {
            foreach (var collectionName in (MongoDbCollectionName[]) Enum.GetValues(typeof(MongoDbCollectionName)))
            {
                var collections = await db.ListCollectionsAsync(new ListCollectionsOptions
                    {Filter = new BsonDocument("name", collectionName.ToString())});

                if (!await collections.AnyAsync())
                    await db.CreateCollectionAsync(collectionName.ToString(),
                        new CreateCollectionOptions() {Collation = new Collation(locale, numericOrdering: true)});

                IIndexBuilder builder;
                switch (collectionName)
                {
                    case MongoDbCollectionName.AddressTransactions:
                        builder = new AddressTransactionIndexBuilder(db);
                        break;
                    case MongoDbCollectionName.Blocks:
                        builder = new BlockIndexBuilder(db);
                        break;
                    case MongoDbCollectionName.Contracts:
                        builder = new ContractIndexBuilder(db);
                        break;
                    case MongoDbCollectionName.Transactions:
                        builder = new TransactionIndexBuilder(db);
                        break;
                    case MongoDbCollectionName.TransactionLogs:
                        builder = new TransactionLogIndexBuilder(db);
                        break;
                    case MongoDbCollectionName.TransactionVMStacks:
                        builder = new TransactionVmStackIndexBuilder(db);
                        break;
                    default:
                        return;
                }

                builder.EnsureIndexes();
            }
        }

        public async Task DeleteAllCollections(IMongoDatabase db)
        {
            foreach (var collectionName in (MongoDbCollectionName[]) Enum.GetValues(typeof(MongoDbCollectionName)))
            {
                await db.DropCollectionAsync(collectionName.ToString());
            }
        }

        public IAddressTransactionRepository CreateAddressTransactionRepository() => new AddressTransactionRepository(_client, _databaseName);
        public IBlockRepository CreateBlockRepository() => new BlockRepository(_client, _databaseName);
        public IContractRepository CreateContractRepository() => new ContractRepository(_client, _databaseName);
        public ITransactionLogRepository CreateTransactionLogRepository() => new TransactionLogRepository(_client, _databaseName);
        public ITransactionRepository CreateTransactionRepository() => new TransactionRepository(_client, _databaseName);
        public ITransactionVMStackRepository CreateTransactionVmStackRepository() => new TransactionVMStackRepository(_client, _databaseName);
    }
}