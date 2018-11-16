using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Web3Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Samples
{
    public class BlockAndTransactionEnumeration
    {  
        public class SimpleTransactionHandler : ITransactionHandler
        {
            public List<TransactionWithReceipt> TransactionsHandled = new List<TransactionWithReceipt>();
            public List<ContractCreationTransaction> ContractCreationsHandled = new List<ContractCreationTransaction>();

            public Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction)
            {
                ContractCreationsHandled.Add(contractCreationTransaction);
                return Task.CompletedTask;
            }

            public Task HandleTransactionAsync(TransactionWithReceipt transactionWithReceipt)
            {
                TransactionsHandled.Add(transactionWithReceipt);
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task Run()
        {
            var web3Wrapper = new Web3Wrapper("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");
            var transactionHandler = new SimpleTransactionHandler();
            var handlers = new HandlerContainer{ TransactionHandler = transactionHandler};

            var blockProcessor = BlockProcessorFactory.Create(
                web3Wrapper, 
                handlers, 
                processTransactionsInParallel: false);

            var processingStrategy = new ProcessingStrategy(blockProcessor);
            var blockchainProcessor = new BlockchainProcessor(processingStrategy);

            var result = await blockchainProcessor.ExecuteAsync(2830144, 2830145);

            Assert.True(result);
            Assert.Equal(20, transactionHandler.TransactionsHandled?.Count);
            Assert.Equal(5, transactionHandler.ContractCreationsHandled?.Count);
        }
    }
}
