using System;
using Nethereum.BlockchainStore.MongoDb.Bootstrap;
using Nethereum.BlockchainStore.Test.Base.RepositoryTests;
using Microsoft.Configuration.Utils;
using Xunit;

namespace Nethereum.BlockchainStore.MongoDb.Tests
{
    public class MongoDbFixture : IDisposable
    {
        public static readonly string[] CommandLineArgs = new string[] {};
        public const string MongoDbConnectionString = "mongodb://localhost:27017";

        public MongoDbFixture()
        {
            ConfigurationUtils.SetEnvironmentAsDevelopment();
            var appConfig = ConfigurationUtils.Build(CommandLineArgs);

            appConfig.SetMongoDbConnectionString(MongoDbConnectionString);

            Factory = MongoDbRepositoryFactory.Create(appConfig, deleteAllExistingCollections: true);
        }

        public MongoDbRepositoryFactory Factory { get; }

        public void Dispose()
        {
            Factory?.DeleteDatabase().Wait();
        }
    }

    [CollectionDefinition("MongoDbFixture")]
    public class MongoDbFixtureCollection : ICollectionFixture<MongoDbFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [Collection("MongoDbFixture")]
    public class MongoDbRepositoryTests: RepositoryLayerTestBase
    {
        public MongoDbRepositoryTests(MongoDbFixture fixture) : base(fixture.Factory){}
    }
}
