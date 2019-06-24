using Common.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Logging.Utils;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.PerformanceTests
{

    class Program
    {
        private static readonly ILog Log = ApplicationLogging
            .CreateConsoleLogger<Program>()
            .ToILog();

        public const string MAIN_NET = "https://mainnet.infura.io/v3/7238211010344719ad14a89db874158c";
        public const string RINKEBY = "https://rinkeby.infura.io/v3/7238211010344719ad14a89db874158c";

        static async Task Main(string[] args)
        {
            try
            {

                Log.Info("Starting");

                var test = new WritingTransfersToTheConsole(
                    MAIN_NET, Log, 
                    numberOfBlocksToProcess: 1000, 
                    maxDuration: TimeSpan.FromHours(2), 
                    maxBlocksPerBatch: 100);

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
