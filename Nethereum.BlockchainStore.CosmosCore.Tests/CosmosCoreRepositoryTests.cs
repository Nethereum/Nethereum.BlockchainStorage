using System;
using Nethereum.BlockchainStore.CosmosCore.Bootstrap;
using Nethereum.BlockchainStore.Test.Base.RepositoryTests;
using Xunit;

namespace Nethereum.BlockchainStore.CosmosCore.Tests
{
    public class CosmosFixture : IDisposable
    {
        public static readonly string[] CommandLineArgs = new string[] {};
        public static readonly string UserSecretsId = "Nethereum.BlockchainStore.CosmosCore.UserSecrets.UnitTests";

        public CosmosFixture()
        {
            ConfigurationUtils.SetEnvironment("development");
            var appConfig = ConfigurationUtils.Build(CommandLineArgs, UserSecretsId);
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
