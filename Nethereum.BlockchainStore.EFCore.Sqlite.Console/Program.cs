using Nethereum.BlockchainStore.Processing;

namespace Nethereum.BlockchainStore.EFCore.Sqlite.Console
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var configuration = ProcessorConfigurationPresets.Get(args);
            var contextFactory = new SqliteBlockchainDbContextFactory(configuration.GetConnectionString());
            var repositoryFactory = new BlockchainStoreRepositoryFactory(contextFactory);
            return ProcessorConsole.Execute(repositoryFactory, configuration).Result;
        }
    }
}
