using System;
using System.Diagnostics;
using Nethereum.BlockchainStore.Bootstrap;

namespace Nethereum.BlockchainStore.Processing.Console
{
    public class Program
    {
        private static readonly string prefix = "Rinkeby";
        private static readonly string connectionString = "DefaultEndpointsProtocol=https;AccountName=davewhiffin;AccountKey=uy2NcHMeK2emMTt5iFcBm/SvkQYi8IEfXKF+L6kGOxEffjRvlUUUYxXPECD+rVIRW4FbEB6MF9/jSuBJaUve7Q==;EndpointSuffix=core.windows.net";

        public static int Main(string[] args)
        {
            var repositoryFactory = new CloudTableSetup(connectionString, prefix);
            var presetName = args?.Length == 0 ? ProcessorConfigurationPresets.Default : args[0];
            var configuration = ProcessorConfigurationPresets.Get(presetName);
            return ProcessorConsole.Execute(args, repositoryFactory, configuration).Result;
        }
    }
}