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
        private static PersistenceStrategy _strategy;

        public static async Task<int> Execute(
            IBlockchainStoreRepositoryFactory repositoryFactory, 
            BlockchainSourceConfiguration configuration,
            FilterContainer filterContainer = null,
            bool useGeth = false)
        {
            IWeb3Wrapper web3 = new Web3Wrapper(
                useGeth 
                    ? new Web3Geth(configuration.BlockchainUrl) 
                    : new Web3.Web3(configuration.BlockchainUrl));

            using (_strategy = new PersistenceStrategy(
                repositoryFactory, filterContainer, minimumBlockNumber: configuration.MinimumBlockNumber ?? 0))
            {
                var blockProcessor = new BlockProcessorFactory()
                    .Create(web3, new VmStackErrorCheckerWrapper(), _strategy, configuration.PostVm);

                blockProcessor.ProcessTransactionsInParallel = configuration.ProcessBlockTransactionsInParallel;

                var blockchainProcessor = new BlockchainProcessor(_strategy, blockProcessor);
                
                //this should not really be necessary
                //but without it, when the process is killed early, some csv records where not being flushed
                AppDomain.CurrentDomain.ProcessExit += (s, e) =>
                {
                    _strategy?.Dispose();
                };

                var stopWatch = Stopwatch.StartNew();

                var result = await blockchainProcessor.ExecuteAsync(configuration.FromBlock, configuration.ToBlock)
                    .ConfigureAwait(false);

                System.Console.WriteLine("Duration: " + stopWatch.Elapsed);

                Debug.WriteLine($"Finished With Success: {result}");
                System.Console.WriteLine("Finished. Success:" + result);
                System.Console.ReadLine();

                return result ? 0 : 1;
                
            }
        }
    }
}
