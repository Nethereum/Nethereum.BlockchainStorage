using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Web3Abstractions;
using Nethereum.Configuration;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Nethereum.BlockchainProcessing.Samples
{
    public class BlockAndTransactionEnumeration
    {  
        public class SimpleTransactionHandler : ITransactionHandler
        {
            public Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction)
            {
                Console.WriteLine($"(Handling Contract Creation Transaction) Block:{contractCreationTransaction.Transaction.BlockNumber.Value}, Hash:{contractCreationTransaction.Transaction.TransactionHash}");
                return Task.CompletedTask;
            }

            public Task HandleTransactionAsync(TransactionWithReceipt transactionWithReceipt)
            {
                Console.WriteLine($"(Handling Transaction) Block:{transactionWithReceipt.Transaction.BlockNumber.Value}, Hash:{transactionWithReceipt.Transaction.TransactionHash}");
                return Task.CompletedTask;
            }
        }

        public async Task Run()
        {
            ApplicationLogging.LoggerFactory.AddConsole(includeScopes: true);

            var targetBlockchain = new BlockchainSourceConfiguration(
                blockchainUrl: "https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60",
                name: "rinkeby") {FromBlock = 2830143, ToBlock = 2830243};
            
            var web3Wrapper = new Web3Wrapper(targetBlockchain.BlockchainUrl);
            var handlers = new HandlerContainer{ TransactionHandler = new SimpleTransactionHandler()};

            var blockProcessor = new BlockProcessorFactory().Create(
                web3Wrapper, 
                handlers, 
                processTransactionsInParallel: false);

            var processingStrategy = new ProcessingStrategy(blockProcessor);
            var blockchainProcessor = new BlockchainProcessor(processingStrategy);

            await blockchainProcessor.ExecuteAsync
                (targetBlockchain.FromBlock, targetBlockchain.ToBlock);
        }
    }
}
