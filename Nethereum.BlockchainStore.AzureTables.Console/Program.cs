using System;
using System.Diagnostics;
using Nethereum.BlockchainStore.Bootstrap;

namespace Nethereum.BlockchainStore.Processing.Console
{
    public class Program
    {
        private static readonly string prefix = "Rinkeby";
        private static readonly string connectionString = "<put your connection string here>";

        public static int Main(string[] args)
        {
            var repositoryFactory = new CloudTableSetup(connectionString, prefix);
            var presetName = args?.Length == 0 ? ProcessorConfigurationPresets.Default : args[0];
            var configuration = ProcessorConfigurationPresets.Get(presetName);
            return ProcessorConsole.Execute(args, repositoryFactory, configuration).Result;
        }
    }
}