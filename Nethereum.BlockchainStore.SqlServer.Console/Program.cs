using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nethereum.BlockchainStore.SqlServer.Console
{
    partial class Program
    {

        public static Dictionary<string, ProcessorConfiguration> Configurations = new Dictionary<string, ProcessorConfiguration>
        {
            {
                "localhost",
                new ProcessorConfiguration(
                    blockchainUrl: "http://localhost:8545", 
                    dbServer: "localhost\\SQLEXPRESS01",
                    database: "BlockchainStorage",
                    dbSchema: "localhost",
                    dbUserName: "localhost1", 
                    dbPassword: "MeLLfMA1wBlJCzSGZhkO"){FromBlock = 0, ToBlock = 37}
            },
            {
                "rinkeby",
                new ProcessorConfiguration(
                    blockchainUrl: "https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60", 
                    dbServer: "localhost\\SQLEXPRESS01",
                    database: "BlockchainStorage",
                    dbSchema: "rinkeby",
                    dbUserName: "rinkeby1", 
                    dbPassword: "rzNk9PyskZg0jLIl"){FromBlock = 2688459, ToBlock = 2788459}
            }
        };

        private static void Main(string[] args)
        {

            new BlockchainDbContextDesignTimeFactory().CreateDbContext(new string[]{});

            string configurationName = args?.Length == 0 ? "localhost" : args[0];

            if (!Configurations.ContainsKey(configurationName))
            {
                throw new Exception($"Unknown configuration name - '{configurationName}'");
            }

            var configuration = Configurations[configurationName];

            var proc = new StorageProcessor(configuration.BlockchainUrl, configuration.ConnectionString, configuration.Schema, configuration.PostVm);
            proc.Init().Wait();
            var result = proc.ExecuteAsync(configuration.FromBlock, configuration.ToBlock).Result;

            Debug.WriteLine(result);
            System.Console.WriteLine(result);
            System.Console.ReadLine();
        }
    }
}
