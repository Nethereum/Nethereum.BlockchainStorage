using Nethereum.BlockchainStore.Repositories;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Web3Abstractions;
using Nethereum.Geth;

namespace Nethereum.BlockchainStore.Processing
{
    public class ProcessorConsole
    {
        private static StorageProcessor _proc;

        public static async Task<int> Execute(
            IBlockchainStoreRepositoryFactory repositoryFactory, 
            BlockchainSourceConfiguration configuration,
            FilterContainer filterContainer = null,
            bool useGeth = false)
        {
            IWeb3Wrapper web3 = new Web3Wrapper(useGeth ? new Web3Geth(configuration.BlockchainUrl) : new Web3.Web3(configuration.BlockchainUrl));

            using(_proc = new StorageProcessor(
                web3, repositoryFactory, configuration.PostVm, filterContainer)
            {
                MinimumBlockNumber = configuration.MinimumBlockNumber,
                ProcessTransactionsInParallel = configuration.ProcessBlockTransactionsInParallel
            })
            {
                //this should not really be necessary
                //but without it, when the process is killed early, some csv records where not being flushed
                AppDomain.CurrentDomain.ProcessExit += (s, e) =>
                {
                    _proc?.Dispose();
                };

                var stopWatch = Stopwatch.StartNew();

                var result = await _proc.ExecuteAsync(configuration.FromBlock, configuration.ToBlock).ConfigureAwait(false);

                System.Console.WriteLine("Duration: " + stopWatch.Elapsed);

                Debug.WriteLine($"Finished With Success: {result}");
                System.Console.WriteLine("Finished. Success:" + result);
                System.Console.ReadLine();

                return result ? 0 : 1;
            }
        }
    }
}
