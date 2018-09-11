﻿using System;
using Nethereum.BlockchainStore.CosmosCore.Bootstrap;
using Nethereum.BlockchainStore.EFCore;
using Nethereum.BlockchainStore.Processing;

namespace Nethereum.BlockchainStore.CosmosCore.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            ConfigurationUtils.SetEnvironment("development");
            var configuration = ProcessorConfigurationPresets.Get(args);

            var repositoryFactory = CosmosRepositoryFactory.Create(
                args, 
                "Nethereum.BlockchainStore.CosmosCore.UserSecrets");

            return ProcessorConsole.Execute(repositoryFactory, configuration).Result;
        }
    }
}
