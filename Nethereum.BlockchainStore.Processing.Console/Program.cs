using System;
using System.Diagnostics;

namespace Nethereum.BlockchainStore.Processing.Console
{
    internal class Program
    {
        private static readonly string prefix = "Morden";

        private static readonly string connectionString =
            "DefaultEndpointsProtocol=https;AccountName=davewhiffin;AccountKey=5+OvslIhZ/HU8awGFTIYeH9kyrIO8nz0DGZ8qM1V4KFtRzveKo6Yt4argVSqt+bEl7aG9Ayo8Tq3AW6Cy8tVcg==;EndpointSuffix=core.windows.net";

        private static void Main(string[] args)
        {
            //string url = "http://localhost:8545";
            //int start = 500599;
            //int end = 1000000;
            //bool postVm = true;

            var url = args?.Length == 0 ? "http://localhost:8545" : args[0];
            var start = args?.Length > 1 ? Convert.ToInt32(args[1]) : 0;
            var end = args?.Length > 2 ? Convert.ToInt32(args[2]) : 1000000;

            var postVm = false;
            if (args.Length > 3)
                if (args[3].ToLower() == "postvm")
                    postVm = true;

            var proc = new StorageProcessor(url, connectionString, prefix, postVm);
            proc.Init().Wait();
            var result = proc.ExecuteAsync(start, end).Result;

            Debug.WriteLine(result);
            System.Console.WriteLine(result);
            System.Console.ReadLine();
        }
    }
}