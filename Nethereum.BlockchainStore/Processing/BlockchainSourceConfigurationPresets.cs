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
            return configuration;
        }

        public static BlockchainSourceConfiguration Get(string name)
        {
            if(!All.ContainsKey(name))
                throw new Exception($"There is no preset configuration for '{name}'");

            return All[name];
        }
    }
}
