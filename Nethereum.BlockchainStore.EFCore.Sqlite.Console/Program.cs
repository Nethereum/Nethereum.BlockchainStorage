using Nethereum.BlockchainStore.EFCore.Processor;

namespace Nethereum.BlockchainStore.EFCore.Sqlite.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var presetName = args?.Length == 0 ? ProcessorConfigurationPresets.Default : args[0];
            var configuration = ProcessorConfigurationPresets.Get(presetName);
            var contextFactory = new BlockchainDbContextFactory(configuration.GetConnectionString());
            return ProcessorConsole.Execute(args, contextFactory, configuration).Result;
        }
    }
}
