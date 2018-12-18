using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Web3Abstractions;
using Nethereum.Geth;
using Nethereum.RPC.Eth.DTOs;
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
            public List<StructLog> Calls = new List<StructLog>();

            public Task HandleAsync(TransactionVmStack transactionVmStack)
            {
                var contractsCalled = transactionVmStack.GetInterContractCalls();
                Calls.AddRange(contractsCalled);
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
            var contractAddresses = new[]
            {"0x786a30e1ab0c58303c85419b9077657ad4fdb0ea","0xd0828aeb00e4db6813e2f330318ef94d2bba2f60","0x6c498f0f83d0bbec758ee7f23e13c9ee522a4c8f","0x243e72b69141f6af525a9a5fd939668ee9f2b354","0x2a212f50a2a020010ea88cc33fc4c333e6a5c5c4" };

            const ulong FromBlock = 57;
            const ulong ToBlock = 57;

            var web3geth = new Web3Geth();
            var web3Wrapper = new Web3Wrapper(web3geth);
            var transactionHandler = new SimpleTransactionHandler();
            var vmStackHandler = new VmStackHandler();
            var contractHandler = new ContractHandler();

            foreach (var contractAddress in contractAddresses)
            {
                contractHandler.ContractAddresses.Add(contractAddress);
            }

            var handlers = new HandlerContainer{ ContractHandler = contractHandler, TransactionHandler = transactionHandler, TransactionVmStackHandler = vmStackHandler};

            var blockProcessor = BlockProcessorFactory.Create(
                web3Wrapper, 
                handlers, 
                processTransactionsInParallel: false,
                postVm: true);

            var processingStrategy = new ProcessingStrategy(blockProcessor){ };

            var blockchainProcessor = new BlockchainProcessor(processingStrategy);

            var result = await blockchainProcessor.ExecuteAsync(FromBlock, ToBlock);

            Assert.True(result);
        }
    }
}
