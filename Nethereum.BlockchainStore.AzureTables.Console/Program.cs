﻿using System;
using System.Diagnostics;

namespace Nethereum.BlockchainStore.Processing.Console
{
    internal class Program
    {
        private static readonly string prefix = "Morden";

        private static readonly string connectionString = "DefaultEndpointsProtocol=https;AccountName=davewhiffin;AccountKey=uy2NcHMeK2emMTt5iFcBm/SvkQYi8IEfXKF+L6kGOxEffjRvlUUUYxXPECD+rVIRW4FbEB6MF9/jSuBJaUve7Q==;EndpointSuffix=core.windows.net";

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