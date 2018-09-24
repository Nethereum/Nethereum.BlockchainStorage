using Nethereum.BlockchainStore.Processing;

namespace Nethereum.BlockchainStore.EFCore.Sqlite.Console
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var appConfig = ConfigurationUtils.Build(args, userSecretsId: "Nethereum.BlockchainStorage.EFCore.Sqlite");
            var blockchainSourceConfiguration = BlockchainSourceConfigurationPresets.Get(appConfig);
            var contextFactory = new SqliteBlockchainDbContextFactory(appConfig.GetBlockchainStorageConnectionString());
            var repositoryFactory = new BlockchainStoreRepositoryFactory(contextFactory);
            return ProcessorConsole.Execute(repositoryFactory, blockchainSourceConfiguration).Result;
        }
    }
}
