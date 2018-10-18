using System;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainStore.CosmosCore.Bootstrap;
using Nethereum.BlockchainStore.Processing;
using Nethereum.Configuration;

namespace Nethereum.BlockchainStore.CosmosCore.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var appConfig = ConfigurationUtils
                .Build(args, userSecretsId: "Nethereum.BlockchainStore.CosmosCore.UserSecrets")
                .AddConsoleLogging();

            var configuration = BlockchainSourceConfigurationFactory.Get(appConfig);
            var repositoryFactory = CosmosRepositoryFactory.Create(appConfig);
            return ProcessorConsole.Execute(repositoryFactory, configuration).Result;
        }
    }
}
