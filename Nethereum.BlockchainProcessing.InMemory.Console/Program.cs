﻿using Nethereum.BlockchainStore;
using Nethereum.BlockchainStore.Processing;
using Nethereum.BlockchainStore.Web3Abstractions;

namespace Nethereum.BlockchainProcessing.InMemory.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine(string.Join(" ", args));

            var appConfig = ConfigurationUtils.Build(args);
            var targetBlockchain = BlockchainSourceConfigurationPresets.Get(appConfig);

            System.Console.WriteLine($"Target Blockchain: {targetBlockchain.Name}, {targetBlockchain.BlockchainUrl}");

            var filters = new FilterContainer();

            var strategy =
                new SampleInMemoryProcessor.InMemoryProcessingStrategy(
                    (msg) => System.Console.WriteLine(msg),
                    filters);

            var web3 = new Web3Wrapper(targetBlockchain.BlockchainUrl);

            var blockProcessor = new BlockProcessorFactory().Create(web3, strategy);
            blockProcessor.ProcessTransactionsInParallel = false;

            var blockchainProcessor = new BlockchainProcessor(strategy, blockProcessor);

            blockchainProcessor.ExecuteAsync(targetBlockchain.MinimumBlockNumber, targetBlockchain.ToBlock).Wait();
        }
    }
}