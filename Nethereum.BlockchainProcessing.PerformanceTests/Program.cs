using Common.Logging;
using Microsoft.Extensions.Logging;
using Nethereum.Logging;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.PerformanceTests
{

    class Program
    {
        private static readonly ILog Log = ApplicationLogging
            .CreateConsoleLogger<Program>()
            .ToILog();

        static async Task Main(string[] args)
        {
            try
            {

                Log.Info("Starting");

                var test = new WritingTransfersToTheConsole(Log, numberOfBlocksToProcess: 171_000, maxDuration: TimeSpan.FromHours(2), maxBlocksPerBatch: 1000);
                //var test = new WritingTransfersToTheAzureStorage(
                //    Config.AzureConnectionString, 
                //    "perfTest", 
                //    numberOfBlocksToProcess: 171_000, 
                //    maxDuration: TimeSpan.FromHours(5), 
                //    maxBlocksPerBatch: 100);
                //WritingTransfersToTheAzureStorage
                await test.RunTestAsync();

                Log.Info("Finished");
            }
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
            }

            Console.ReadLine();
        }
    }
}
