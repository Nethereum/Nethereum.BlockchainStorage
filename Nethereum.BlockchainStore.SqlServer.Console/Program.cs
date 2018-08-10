using System;
using System.Diagnostics;

namespace Nethereum.BlockchainStore.SqlServer.Console
{
    class Program
    {
        private static readonly string schema = "localhost";

        private static readonly string connectionString = BlockchainDbContextDesignTimeFactory.connectionString;

        private static void Main(string[] args)
        {
            //2788459
            //var url = args?.Length == 0 ? "http://localhost:8545" : args[0];
            var url = args?.Length == 0 ? "https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60" : args[0];
            var start = args?.Length > 1 ? Convert.ToInt32(args[1]) : 2688459;
            var end = args?.Length > 2 ? Convert.ToInt32(args[2]) : 2788459;

            var postVm = false;
            if (args.Length > 3)
                if (args[3].ToLower() == "postvm")
                    postVm = true;

            var proc = new StorageProcessor(url, connectionString, schema, postVm);
            proc.Init().Wait();
            var result = proc.ExecuteAsync(start, end).Result;

            Debug.WriteLine(result);
            System.Console.WriteLine(result);
            System.Console.ReadLine();
        }
    }
}
