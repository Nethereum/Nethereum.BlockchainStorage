using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Web3Abstractions;
using Nethereum.Geth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Samples
{
    public class InterContractHandling
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

        public class VmStackHandler : ITransactionVMStackHandler
        {
            public List<TransactionVmStack> Stacks = new List<TransactionVmStack>();

            public Task HandleAsync(TransactionVmStack transactionVmStack)
            {
                var contractsCalled = transactionVmStack.GetContractsCalled();

                Stacks.Add(transactionVmStack);
                return Task.CompletedTask;
            }
        }

        public class ContractHandler : IContractHandler
        {
            public HashSet<string> ContractAddresses = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            public Task<bool> ExistsAsync(string contractAddress)
            {
                return Task.FromResult(ContractAddresses.Contains(contractAddress));
            }

            public Task HandleAsync(ContractTransaction contractTransaction)
            {
                ContractAddresses.Add(contractTransaction.ContractAddress);
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task Run()
        {
            var web3geth = new Web3Geth();
            var web3Wrapper = new Web3Wrapper(web3geth);
            var transactionHandler = new SimpleTransactionHandler();
            var vmStackHandler = new VmStackHandler();
            var contractHandler = new ContractHandler();
            contractHandler.ContractAddresses.Add("0x243e72b69141f6af525a9a5fd939668ee9f2b354");
            contractHandler.ContractAddresses.Add("0x2a212f50a2a020010ea88cc33fc4c333e6a5c5c4");
            contractHandler.ContractAddresses.Add("0xd0828aeb00e4db6813e2f330318ef94d2bba2f60");

            var handlers = new HandlerContainer{ ContractHandler = contractHandler, TransactionHandler = transactionHandler, TransactionVmStackHandler = vmStackHandler};

            var blockProcessor = BlockProcessorFactory.Create(
                web3Wrapper, 
                handlers, 
                processTransactionsInParallel: false,
                postVm: true);

            var processingStrategy = new ProcessingStrategy(blockProcessor){ };

            var blockchainProcessor = new BlockchainProcessor(processingStrategy);

            var result = await blockchainProcessor.ExecuteAsync(20, 20);

            Assert.True(result);
        }
    }
}
