using Nethereum.BlockchainStore.Processing;

namespace Nethereum.BlockchainStore.EFCore.SqlServer.Console
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var configuration = ProcessorConfigurationPresets.Get(args);
            var contextFactory = new BlockchainDbContextFactory(configuration.GetConnectionString(), configuration.Schema);
            var repositoryFactory = new BlockchainStoreRepositoryFactory(contextFactory);
            return ProcessorConsole.Execute(repositoryFactory, configuration).Result;
        }
    }
}
