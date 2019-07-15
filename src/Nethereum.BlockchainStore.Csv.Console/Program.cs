using Microsoft.Extensions.Logging;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainStore.Processing;
using Microsoft.Configuration.Utils;
using Microsoft.Logging.Utils;

namespace Nethereum.BlockchainStore.Csv.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var log = ApplicationLogging.CreateConsoleLogger<Program>().ToILog();

            var appConfig = ConfigurationUtils
                .Build(args, userSecretsId: "Nethereum.BlockchainStore.Csv.UserSecrets");

            var configuration = BlockchainSourceConfigurationFactory.Get(appConfig);
            var outputPath = appConfig["CsvOutputPath"];
            var repositoryFactory = new CsvBlockchainStoreRepositoryFactory(outputPath);
            return StorageProcessorConsole.Execute(repositoryFactory, configuration, log: log).Result;
        }
    }
}
