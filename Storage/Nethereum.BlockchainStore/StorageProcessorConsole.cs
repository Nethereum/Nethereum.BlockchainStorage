using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Geth;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processing
{
    public class StorageProcessorConsole
    {
        public static async Task<int> Execute(
            IBlockchainStoreRepositoryFactory repositoryFactory, 
            BlockchainSourceConfiguration configuration,
            FilterContainer filterContainer = null,
            bool useGeth = false)
        {
            IBlockchainProxyService web3 = new BlockchainProxyService(
                useGeth 
                    ? new Web3Geth(configuration.BlockchainUrl) 
                    : new Web3.Web3(configuration.BlockchainUrl));

            using(var repositoryHandlerContext = new RepositoryHandlerContext(repositoryFactory))
            {
                var blockProcessor = BlockProcessorFactory
                        .Create(
                            web3, 
                            repositoryHandlerContext.Handlers, 
                            postVm: configuration.PostVm,
                            processTransactionsInParallel: configuration.ProcessBlockTransactionsInParallel);

                var storageProcessingStrategy = new StorageProcessingStrategy(
                    repositoryHandlerContext, blockProcessor)
                {
                    MinimumBlockNumber = configuration.MinimumBlockNumber ?? 0,
                    MinimumBlockConfirmations = configuration.MinimumBlockConfirmations ?? 0
                };
                
                var blockchainProcessor = new BlockchainProcessor(storageProcessingStrategy);

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
