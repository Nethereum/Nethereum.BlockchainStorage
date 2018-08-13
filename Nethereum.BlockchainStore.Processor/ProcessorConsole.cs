using Nethereum.BlockchainStore.EFCore;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processor
{
    public class ProcessorConsole
    {
        public static async Task<int> Execute(string[] args, IBlockchainDbContextFactory contextFactory, ProcessorConfiguration configuration)
        {
            var proc = new StorageProcessor(configuration.BlockchainUrl, contextFactory, configuration.PostVm);
            await proc.Init();

            var stopWatch = Stopwatch.StartNew();

            proc.ProcessTransactionsInParallel = true;

            var result = await proc.ExecuteAsync(configuration.FromBlock, configuration.ToBlock).ConfigureAwait(false);

            var duration = stopWatch.Elapsed;

            System.Console.WriteLine("Duration: " + duration);

            Debug.WriteLine($"Finished With Success: {result}");
            System.Console.WriteLine("Finished. Success:" + result);
            System.Console.ReadLine();

            return result ? 0 : 1;
        }
    }
}
