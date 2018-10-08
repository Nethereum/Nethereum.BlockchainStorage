# Nethereum.BlockchainProcessing.InMemory.Console

A sample to demonstrate reading sequentially from the blockchain and configuring handlers to invoke custom behaviour.

The example handlers write to the console each time they are invoked.  They demonstrate how simple it is to read from the blockchain.  

Writing your own handlers is just a matter of creating your own class that implements the relevant handler interface.

## Filters

You can use a pre defined filter and create your own.  For instance you may only be interested in transactions sent to or from a specific address. 

When filters are provided, the handlers are only invoked if one or more of the filters match.

You could ignore filters and implement the criteria in the handlers.  However, using filters will minimise unecessary processing - such as retrieving a transaction receipt for a transaction you are not interested in.

``` csharp
using Nethereum.BlockchainStore;
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
            var targetBlockchain = BlockchainSourceConfigurationFactory.Get(appConfig);

            System.Console.WriteLine($"Target Blockchain: {targetBlockchain.Name}, {targetBlockchain.BlockchainUrl}");

            var filters = new FilterContainer();

            //Example Filters 
            //These are singular - but the filter container can accept multiple filters
            //When using multiple filters, only one has to match 
            //var filters = new FilterContainer(new TransactionFilter((tx) => tx.Value.Value > 0 && tx.From == "<some address>"));
            //var filters = new FilterContainer(TransactionReceiptFilter.OnlyNewContracts());
            //var filters = new FilterContainer(TransactionFilter.FromAndTo("<from>", "<to>"));
            //var filters = new FilterContainer(TransactionReceiptFilter.OnlyNewContracts());

            var strategy = new ProcessingStrategy
            {
                Filters = filters,
                BlockHandler = new InMemoryBlockHandler(System.Console.WriteLine),
                TransactionHandler = new InMemoryTransactionHandler(System.Console.WriteLine),
                TransactionLogHandler = new InMemoryTransactionLogHandler(System.Console.WriteLine),
                TransactionVmStackHandler = new InMemoryTransactionVmStackHandler(System.Console.WriteLine),
                ContractHandler = new InMemoryContractHandler(System.Console.WriteLine)
            };

            var web3 = new Web3Wrapper(targetBlockchain.BlockchainUrl);

            var blockProcessorFactory = new BlockProcessorFactory();
            var blockProcessor = blockProcessorFactory.Create(web3, strategy, processTransactionsInParallel: false);
            var blockchainProcessor = new BlockchainProcessor(strategy, blockProcessor);

            blockchainProcessor.ExecuteAsync
                (targetBlockchain.MinimumBlockNumber, targetBlockchain.ToBlock)
                .Wait();
        }
    }
}

```