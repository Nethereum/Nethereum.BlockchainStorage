using Nethereum.BlockchainStore.Processing;

namespace Nethereum.BlockchainStore.EFCore.SqlServer.Console
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var presetName = args?.Length == 0 ? ProcessorConfigurationPresets.Default : args[0];
            var configuration = ProcessorConfigurationPresets.Get(presetName);
            var contextFactory = new BlockchainDbContextFactory(configuration.GetConnectionString(), configuration.Schema);
            var repositoryFactory = new BlockchainStoreRepositoryFactory(contextFactory);
            return ProcessorConsole.Execute(args, repositoryFactory, configuration).Result;
        }
    }
}
