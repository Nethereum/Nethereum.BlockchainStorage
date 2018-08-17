﻿using System.Diagnostics;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Repositories;

namespace Nethereum.BlockchainStore.Processing
{
    public class ProcessorConsole
    {
        public static async Task<int> Execute(IBlockchainStoreRepositoryFactory repositoryFactory, ProcessorConfiguration configuration)
        {
            var proc = new StorageProcessor(configuration.BlockchainUrl, repositoryFactory, configuration.PostVm)
            {
                MinimumBlockNumber = configuration.MinimumBlockNumber,
                ProcessTransactionsInParallel = configuration.ProcessBlockTransactionsInParallel
            };

            var stopWatch = Stopwatch.StartNew();

            var result = await proc.ExecuteAsync(configuration.FromBlock, configuration.ToBlock).ConfigureAwait(false);

            System.Console.WriteLine("Duration: " + stopWatch.Elapsed);

            Debug.WriteLine($"Finished With Success: {result}");
            System.Console.WriteLine("Finished. Success:" + result);
            System.Console.ReadLine();

            return result ? 0 : 1;
        }
    }
}
