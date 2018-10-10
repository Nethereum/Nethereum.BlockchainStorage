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

            var appConfig = ConfigurationUtils.Build(args).AddConsoleLogging();

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
                ContractHandler = new InMemoryContractHandler(System.Console.WriteLine),
                MinimumBlockConfirmations = 6
            };

            var web3 = new Web3Wrapper(targetBlockchain.BlockchainUrl);

            var blockProcessorFactory = new BlockProcessorFactory();
            var blockProcessor = blockProcessorFactory.Create(web3, strategy, processTransactionsInParallel: false);
            var blockchainProcessor = new BlockchainProcessor(strategy, blockProcessor);

            blockchainProcessor.ExecuteAsync
                (targetBlockchain.FromBlock, targetBlockchain.ToBlock)
                .Wait();
        }
    }
}
