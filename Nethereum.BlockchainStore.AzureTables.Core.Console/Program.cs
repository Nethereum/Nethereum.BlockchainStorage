using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using Nethereum.BlockchainStore.Processing;

namespace Nethereum.BlockchainStore.AzureTables.Core.Console
{
    class Program
    {
        private const string ConnectionStringKey = "AzureStorageConnectionString";

        public static int Main(string[] args)
        {
            var appConfig = ConfigurationUtils.Build(args, userSecretsId: "Nethereum.BlockchainStore.AzureTables");
            var configuration = BlockchainSourceConfigurationFactory.Get(appConfig);

            var connectionString = appConfig[ConnectionStringKey];

            if (string.IsNullOrEmpty(connectionString))
                throw ConfigurationUtils.CreateKeyNotFoundException(ConnectionStringKey);

            var repositoryFactory = new CloudTableSetup(connectionString, configuration.Name);
            return ProcessorConsole.Execute(repositoryFactory, configuration).Result;
        }
    }
}
