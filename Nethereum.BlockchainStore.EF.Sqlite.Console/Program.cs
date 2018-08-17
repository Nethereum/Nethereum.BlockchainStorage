using Nethereum.BlockchainStore.Processing;

namespace Nethereum.BlockchainStore.EF.Sqlite.Console
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var contextFactory = new SqliteBlockchainDbContextFactory("BlockchainDbContext");
            var repositoryFactory = new BlockchainStoreRepositoryFactory(contextFactory);
            var configuration = ProcessorConfigurationPresets.Get(args);
            return ProcessorConsole.Execute(repositoryFactory, configuration).Result;
        }
    }
}
