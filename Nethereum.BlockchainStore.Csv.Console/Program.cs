using System;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainStore.Processing;
using Nethereum.Configuration;

namespace Nethereum.BlockchainStore.Csv.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var appConfig = ConfigurationUtils
                .Build(args, userSecretsId: "Nethereum.BlockchainStore.CosmosCore.UserSecrets")
                .AddConsoleLogging();

            var configuration = BlockchainSourceConfigurationFactory.Get(appConfig);
            var outputPath = appConfig["CsvOutputPath"];
            var repositoryFactory = new CsvBlockchainStoreRepositoryFactory(outputPath);
            return ProcessorConsole.Execute(repositoryFactory, configuration).Result;
        }
    }
}
