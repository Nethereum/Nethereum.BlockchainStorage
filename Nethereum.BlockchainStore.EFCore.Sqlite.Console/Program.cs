using Nethereum.BlockchainStore.Processing;

namespace Nethereum.BlockchainStore.EFCore.Sqlite.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var presetName = args?.Length == 0 ? ProcessorConfigurationPresets.Default : args[0];
            var configuration = ProcessorConfigurationPresets.Get(presetName);
            var contextFactory = new BlockchainDbContextFactory(configuration.GetConnectionString());
            var repositoryFactory = new BlockchainStoreRepositoryFactory(contextFactory);
            return ProcessorConsole.Execute(args, repositoryFactory, configuration).Result;
        }
    }
}
