using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using System;
using Xunit;

namespace Nethereum.BlockchainStore.AzureTables.Tests.RepositoryTests
{
    public class AzureTablesFixture : IDisposable
    {
        public static readonly string[] CommandLineArgs = new string[] {};
        public static readonly string UserSecretsId = "Nethereum.BlockchainStore.AzureTables";

        public AzureTablesFixture()
        {
            ConfigurationUtils.SetEnvironment("development");
            var appConfig = ConfigurationUtils.Build(CommandLineArgs, UserSecretsId);
            var connectionString = appConfig["AzureStorageConnectionString"];
            Factory = new CloudTableSetup(connectionString, "UnitTest");
        }

        public CloudTableSetup Factory { get; }

        public void Dispose()
        {
            Factory?.DeleteAllTables().Wait();
        }
    }

    [CollectionDefinition("AzureTablesFixture")]
    public class AzureTablesFixtureCollection : ICollectionFixture<AzureTablesFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
