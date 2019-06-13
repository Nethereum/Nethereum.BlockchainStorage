using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockProcessing.Samples
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
            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);
            var transactionHandler = new SimpleTransactionHandler();
            var handlers = new HandlerContainer{ TransactionHandler = transactionHandler};

            var blockProcessor = BlockProcessorFactory.Create(
                web3, 
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
