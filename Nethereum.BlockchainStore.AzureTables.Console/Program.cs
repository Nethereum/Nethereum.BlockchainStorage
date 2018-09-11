using Nethereum.BlockchainStore.Bootstrap;

namespace Nethereum.BlockchainStore.Processing.Console
{
    public class Program
    {
        private static readonly string connectionString = "<put your connection string here>";

        public static int Main(string[] args)
        {
            var presetName = args?.Length == 0 ? ProcessorConfigurationPresets.Default : args[0];
            var configuration = ProcessorConfigurationPresets.Get(presetName);
            var repositoryFactory = new CloudTableSetup(connectionString, configuration.Schema);
            return ProcessorConsole.Execute(repositoryFactory, configuration).Result;
        }
    }
}