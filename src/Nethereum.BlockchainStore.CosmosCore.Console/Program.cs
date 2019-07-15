using Microsoft.Extensions.Logging;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainStore.CosmosCore.Bootstrap;
using Nethereum.BlockchainStore.Processing;
using Microsoft.Configuration.Utils;
using Microsoft.Logging.Utils;

namespace Nethereum.BlockchainStore.CosmosCore.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var log = ApplicationLogging.CreateConsoleLogger<Program>().ToILog();

            var appConfig = ConfigurationUtils
                .Build(args, userSecretsId: "Nethereum.BlockchainStore.CosmosCore.UserSecrets");

            var configuration = BlockchainSourceConfigurationFactory.Get(appConfig);
            var repositoryFactory = CosmosRepositoryFactory.Create(appConfig);
            return StorageProcessorConsole.Execute(repositoryFactory, configuration, log: log).Result;
        }
    }
}
