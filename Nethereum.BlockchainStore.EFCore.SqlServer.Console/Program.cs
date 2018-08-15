using Nethereum.BlockchainStore.EFCore.Processor;

namespace Nethereum.BlockchainStore.EFCore.SqlServer.Console
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var presetName = args?.Length == 0 ? ProcessorConfigurationPresets.Default : args[0];
            var configuration = ProcessorConfigurationPresets.Get(presetName);

            var contextFactory = new BlockchainDbContextFactory(configuration.GetConnectionString(), configuration.Schema);

            return ProcessorConsole.Execute(args, contextFactory, configuration).Result;
        }
    }
}
