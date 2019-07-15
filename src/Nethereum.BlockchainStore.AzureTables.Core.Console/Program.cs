using Microsoft.Configuration.Utils;
using Microsoft.Extensions.Configuration;
using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using Nethereum.BlockchainStore.Console;
using System.Threading;

namespace Nethereum.BlockchainStore.AzureTables.Core.Console
{
    class Program
    {
        private const string ConnectionStringKey = "AzureStorageConnectionString";

        public static int Main(string[] args)
        {
            var storageProcessorConsole = new StorageProcessorConsole<AzureTablesRepositoryFactory>(
                args,
                "Nethereum.BlockchainStore.AzureTables",
                CreateRepositoryFactory
            );

            return storageProcessorConsole.ExecuteAsync(
                new CancellationToken())
                .Result;

        }

        private static AzureTablesRepositoryFactory CreateRepositoryFactory(IConfigurationRoot config)
        {
            var connectionString = config[ConnectionStringKey];

            if (string.IsNullOrEmpty(connectionString))
                throw ConfigurationUtils.CreateKeyNotFoundException(ConnectionStringKey);

            var tablePrefix = config["DbSchema"] ?? string.Empty;

            return new AzureTablesRepositoryFactory(connectionString, tablePrefix);
        }
    }
}
