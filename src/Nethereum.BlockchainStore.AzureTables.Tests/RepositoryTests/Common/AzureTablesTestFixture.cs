using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using System;
using Nethereum.Microsoft.Configuration.Utils;
using Xunit;

namespace Nethereum.BlockchainStore.AzureTables.Tests.RepositoryTests
{
    public class AzureTablesFixture : IDisposable
    {
        public static readonly string[] CommandLineArgs = new string[] {};
        public static readonly string UserSecretsId = "Nethereum.BlockchainStore.AzureTables";

        public AzureTablesFixture()
        {
            /*
             To run the tests setup the connection string in the user secrets
             In the command line navigate to folder: Nethereum.BlockchainStorage\src\Nethereum.BlockchainStore.AzureTables.Tests
             Set the connection string using the syntax below
             dotnet user-secrets set "AzureStorageConnectionString" "CONNECTION_STRING"
             */

            ConfigurationUtils.SetEnvironmentAsDevelopment();
            var appConfig = ConfigurationUtils.Build(CommandLineArgs, UserSecretsId);
            var connectionString = appConfig["AzureStorageConnectionString"];
            Factory = new AzureTablesRepositoryFactory(connectionString, "UnitTest");
        }

        public AzureTablesRepositoryFactory Factory { get; }

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
