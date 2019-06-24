using Microsoft.Extensions.Logging;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainStore.MongoDb.Bootstrap;
using Nethereum.BlockchainStore.Processing;
using Microsoft.Configuration.Utils;
using Microsoft.Logging.Utils;

namespace Nethereum.BlockchainStore.MongoDb.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var log = ApplicationLogging.CreateConsoleLogger<Program>().ToILog();

            var appConfig = ConfigurationUtils
                .Build(args, userSecretsId: "Nethereum.BlockchainStore.MongoDb.UserSecrets");

            var configuration = BlockchainSourceConfigurationFactory.Get(appConfig);
            var repositoryFactory = MongoDbRepositoryFactory.Create(appConfig);
            return StorageProcessorConsole.Execute(repositoryFactory, configuration, log: log).Result;
        }
    }
}
