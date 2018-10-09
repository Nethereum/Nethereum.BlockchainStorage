using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using Nethereum.BlockchainStore.CosmosCore.Bootstrap;
using Nethereum.BlockchainStore.Test.Base.RepositoryTests;
using Xunit;

namespace Nethereum.BlockchainStore.CosmosCore.Tests
{
    public class CosmosFixture : IDisposable
    {
        public static readonly string[] CommandLineArgs = new string[] {};

        public CosmosFixture()
        {
            ConfigurationUtils.SetEnvironment("development");
            var appConfig = ConfigurationUtils.Build(CommandLineArgs);

            //Azure Cosmos DB emulator settings
            //https://localhost:8081/_explorer/index.html

            appConfig.SetCosmosEndpointUri("https://localhost:8081");
            appConfig.SetCosmosAccessKey(
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");

            Factory = CosmosRepositoryFactory.Create(appConfig, deleteAllExistingCollections: true);
        }

        public CosmosRepositoryFactory Factory { get; }

        public void Dispose()
        {
            Factory?.DeleteDatabase().Wait();
        }
    }

    [CollectionDefinition("CosmosFixture")]
    public class CosmosFixtureCollection : ICollectionFixture<CosmosFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [Collection("CosmosFixture")]
    public class CosmosCoreRepositoryTests: RepositoryLayerTestBase
    {
        public CosmosCoreRepositoryTests(CosmosFixture fixture) : base(fixture.Factory){}

    }
}
