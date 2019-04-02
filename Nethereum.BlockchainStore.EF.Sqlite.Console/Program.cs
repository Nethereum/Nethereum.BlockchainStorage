using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainStore.Processing;
using Nethereum.Configuration;

namespace Nethereum.BlockchainStore.EF.Sqlite.Console
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var appConfig = ConfigurationUtils.Build(args).AddConsoleLogging();
            var contextFactory = new SqliteBlockchainDbContextFactory("BlockchainDbContext");
            var repositoryFactory = new BlockchainStoreRepositoryFactory(contextFactory);
            var configuration = BlockchainSourceConfigurationFactory.Get(appConfig);
            configuration.ProcessBlockTransactionsInParallel = false;
            return StorageProcessorConsole.Execute(repositoryFactory, configuration).Result;
        }
    }
}
