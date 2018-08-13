using System;
using System.Collections.Generic;
using System.Text;

namespace Nethereum.BlockchainStore.SqlServer.Console
{
    public static class ConfigurationPresets
    {
        public static string Default { get; } = "localhost";

        public static Dictionary<string, ProcessorConfiguration> All = new Dictionary<string, ProcessorConfiguration>
        {
            {
                "localhost",
                new ProcessorConfiguration(
                    blockchainUrl: "http://localhost:8545", 
                    dbSchema: "localhost"){FromBlock = 0, ToBlock = 37}
            },
            {
                "rinkeby",
                new ProcessorConfiguration(
                    blockchainUrl: "https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60", 
                    dbSchema: "rinkeby"){FromBlock = 2688459, ToBlock = 2788459}
            }
        };

        public static ProcessorConfiguration Get(string name)
        {
            if(!All.ContainsKey(name))
                throw new Exception($"There is no preset configuration for '{name}'");

            return All[name];
        }
    }
}
