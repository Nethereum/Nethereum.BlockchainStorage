using Nethereum.BlockchainStore.Processing;
using Nethereum.BlockchainStore.Processors.Transactions;

namespace Nethereum.BlockchainStore.EFCore.Sqlite.Console
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var appConfig = ConfigurationUtils
                .Build(args, userSecretsId: "Nethereum.BlockchainStorage.EFCore.Sqlite")
                .AddConsoleLogging();

            var blockchainSourceConfiguration = BlockchainSourceConfigurationFactory.Get(appConfig);
            var contextFactory = new SqliteBlockchainDbContextFactory(appConfig.GetBlockchainStorageConnectionString());
            var repositoryFactory = new BlockchainStoreRepositoryFactory(contextFactory);

            return ProcessorConsole.Execute(repositoryFactory, blockchainSourceConfiguration).Result;
        }
    }
}
