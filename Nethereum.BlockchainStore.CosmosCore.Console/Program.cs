using System;
using Nethereum.BlockchainStore.CosmosCore.Bootstrap;
using Nethereum.BlockchainStore.Processing;

namespace Nethereum.BlockchainStore.CosmosCore.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var appConfig = ConfigurationUtils.Build(args, userSecretsId: "Nethereum.BlockchainStore.CosmosCore.UserSecrets");
            var configuration = BlockchainSourceConfigurationPresets.Get(appConfig);
            var repositoryFactory = CosmosRepositoryFactory.Create(appConfig);
            return ProcessorConsole.Execute(repositoryFactory, configuration).Result;
        }
    }
}
