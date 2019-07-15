using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Nethereum.BlockchainStore.CosmosCore.Bootstrap;
using Nethereum.BlockchainStore.Test.Base.RepositoryTests;
using Nethereum.Configuration;
using Xunit;

namespace Nethereum.BlockchainStore.CosmosCore.Tests
{
    public class CosmosFixture : IDisposable
    {
        public static readonly string[] CommandLineArgs = new string[] {};
        public const string CosmosEmulatorBaseUrl = "https://localhost:8081";
        public const string CosmosAccessKey =
            "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        public static Uri CosmosEmulatorUi = new Uri($"{CosmosEmulatorBaseUrl}/_explorer/index.html");
        public const string EmulatorPath = @"C:\Program Files\Azure Cosmos DB Emulator\CosmosDB.Emulator.exe";

        public CosmosFixture()
        {
            ConfigurationUtils.SetEnvironment("development");
            var appConfig = ConfigurationUtils.Build(CommandLineArgs);

            appConfig.SetCosmosEndpointUri(CosmosEmulatorBaseUrl);
            appConfig.SetCosmosAccessKey(CosmosAccessKey);

            if (!IsEmulatorRunning())
            {
                string message = $"The Cosmos DB Emulator is not running at Uri '{CosmosEmulatorBaseUrl}'";

                if(File.Exists(EmulatorPath))
                    throw new Exception(message);

                throw new Exception($"{message}.  The emulator does not appear to be installed (at least not in the default location '{EmulatorPath}').");
            }

            Factory = CosmosRepositoryFactory.Create(appConfig, deleteAllExistingCollections: true);
        }

        public CosmosRepositoryFactory Factory { get; }

        public void Dispose()
        {
            Factory?.DeleteDatabase().Wait();
        }

        private bool IsEmulatorRunning()
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    using (webClient.OpenRead(CosmosEmulatorUi))
                    {
                        return true;
                    }
                }
            }
            catch (WebException)
            {
                return false;
            }
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
