using Nethereum.BlockchainStore.Bootstrap;
using Nethereum.BlockchainStore.Processing;

namespace Nethereum.BlockchainStore.AzureTables.Console
{
    public class Program
    {
        private static readonly string connectionString = "<put your connection string here>";

        public static int Main(string[] args)
        {
            var presetName = args?.Length == 0 ? BlockchainSourceConfigurationPresets.Default : args[0];
            var configuration = BlockchainSourceConfigurationPresets.Get(presetName);
            var repositoryFactory = new CloudTableSetup(connectionString, configuration.Name);
            return ProcessorConsole.Execute(repositoryFactory, configuration).Result;
        }
    }
}