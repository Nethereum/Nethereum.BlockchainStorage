using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Nethereum.BlockchainStore.Processing
{
    public static class BlockchainSourceConfigurationPresets
    {
        public static string Default { get; } = "localhost";

        public static Dictionary<string, BlockchainSourceConfiguration> All = new Dictionary<string, BlockchainSourceConfiguration>
        {
            {
                "localhost",
                new BlockchainSourceConfiguration(
                    blockchainUrl: "http://localhost:8545", 
                    name: "localhost"){FromBlock = 0}
            },
            {
                "rinkeby",
                new BlockchainSourceConfiguration(
                    blockchainUrl: "https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60", 
                    name: "rinkeby"){MinimumBlockNumber = 2830143}
            }
        };

        public static BlockchainSourceConfiguration Get(IConfigurationRoot config)
        {
            var presetName = config["Blockchain"] ?? Default;
            var configuration = Get(presetName);

            ApplyOverrides(config, configuration);

            return configuration;
        }

        private static void ApplyOverrides(IConfigurationRoot config, BlockchainSourceConfiguration configuration)
        {
            var minBlockNumber = Parse(config, "MinimumBlockNumber");
            var fromBlock = Parse(config, "FromBlock");
            var toBlock = Parse(config, "ToBlock");
            var blockchainUrl = config["BlockchainUrl"];
            var blockchainName = config["Blockchain"];
            var postVm = ParseBool(config, "PostVm");
            var processTransactionsInParallel = ParseBool(config, "ProcessBlockTransactionsInParallel");

            if (minBlockNumber != null)
                configuration.MinimumBlockNumber = minBlockNumber;

            if (fromBlock != null)
                configuration.FromBlock = fromBlock;

            if (toBlock != null)
                configuration.ToBlock = toBlock;

            if (blockchainUrl != null)
                configuration.BlockchainUrl = blockchainUrl;

            if (blockchainName != null)
                configuration.Name = blockchainName;

            if (postVm != null)
                configuration.PostVm = postVm.Value;

            if (processTransactionsInParallel != null)
                configuration.ProcessBlockTransactionsInParallel = processTransactionsInParallel.Value;
        }

        public static long? Parse(IConfigurationRoot config, string name)
        {
            var configVal = config[name];

            return string.IsNullOrEmpty(configVal)
                ? (long?) null
                : long.Parse(configVal);
        }

        public static bool? ParseBool(IConfigurationRoot config, string name)
        {
            var configVal = config[name];

            return string.IsNullOrEmpty(configVal)
                ? (bool?) null
                : bool.Parse(configVal);
        }

        public static BlockchainSourceConfiguration Get(string name)
        {
            if(!All.ContainsKey(name))
                throw new Exception($"There is no preset configuration for '{name}'");

            return All[name];
        }
    }
}
