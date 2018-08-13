using System;
using System.Diagnostics;
using Nethereum.Blockchain.Sqlite;

namespace Nethereum.BlockchainStore.Sqlite.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new BlockchainDbContext();

            var presetName = args?.Length == 0 ? ConfigurationPresets.Default : args[0];
            var configuration = ConfigurationPresets.Get(presetName);

            var proc = new StorageProcessor(configuration.BlockchainUrl, configuration.GetConnectionString(), configuration.Schema, configuration.PostVm);
            proc.Init().Wait();
            var result = proc.ExecuteAsync(configuration.FromBlock, configuration.ToBlock).Result;

            Debug.WriteLine($"Finished With Success: {result}");
            System.Console.WriteLine(result);
            System.Console.ReadLine();
        }
    }
}
