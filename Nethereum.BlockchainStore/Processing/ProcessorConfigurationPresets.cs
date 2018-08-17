using System;
using System.Collections.Generic;

namespace Nethereum.BlockchainStore.Processing
{
    public static class ProcessorConfigurationPresets
    {
        public static string Default { get; } = "dbo";

        public static Dictionary<string, ProcessorConfiguration> All = new Dictionary<string, ProcessorConfiguration>
        {
            {
                "dbo",
                new ProcessorConfiguration(
                    blockchainUrl: "http://localhost:8545", 
                    dbSchema: "dbo"){FromBlock = 0}
            },
            {
                "localhost",
                new ProcessorConfiguration(
                    blockchainUrl: "http://localhost:8545", 
                    dbSchema: "localhost"){FromBlock = 0}
            },
            {
                "rinkeby",
                new ProcessorConfiguration(
                    blockchainUrl: "https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60", 
                    dbSchema: "rinkeby"){MinimumBlockNumber = 2830143}
            }
        };

        public static ProcessorConfiguration Get(string[] consoleArgs)
        {
            var presetName = consoleArgs?.Length == 0 ? Default : consoleArgs[0];
            var configuration = Get(presetName);
            return configuration;
        }

        public static ProcessorConfiguration Get(string name)
        {
            if(!All.ContainsKey(name))
                throw new Exception($"There is no preset configuration for '{name}'");

            return All[name];
        }
    }
}
