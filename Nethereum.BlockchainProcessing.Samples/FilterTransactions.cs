using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processors.Transactions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Xunit;

namespace Nethereum.BlockchainProcessing.Samples
{
    public class FilterTransactions
    {  
        public class SimpleTransactionHandler : ITransactionHandler
        {
            public List<TransactionWithReceipt> TransactionsHandled = new List<TransactionWithReceipt>();
            public List<ContractCreationTransaction> ContractCreationTransactionsHandled = new List<ContractCreationTransaction>();

            public Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction)
            {
                ContractCreationTransactionsHandled.Add(contractCreationTransaction); 
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
            var blockchainProxyService = new BlockchainProxyService("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");
            var transactionHandler = new SimpleTransactionHandler();
            var handlers = new HandlerContainer{ TransactionHandler = transactionHandler};

            //only tx sent to this address
            var transactionFilter = TransactionFilter.To("0xc0e15e11306334258d61fee52a22d15e6c9c59e0");

            var filter = new FilterContainer(transactionFilter);

            var blockProcessor = BlockProcessorFactory.Create(
                blockchainProxyService, 
                handlers, 
                filter,
                processTransactionsInParallel: false);

            var processingStrategy = new ProcessingStrategy(blockProcessor);
            var blockchainProcessor = new BlockchainProcessor(processingStrategy);

            var result = await blockchainProcessor.ExecuteAsync(2830143, 2830153);

            Assert.True(result);
            Assert.Equal(12, transactionHandler.TransactionsHandled.Count);
            Assert.Empty(transactionHandler.ContractCreationTransactionsHandled);
        }
    }
}
