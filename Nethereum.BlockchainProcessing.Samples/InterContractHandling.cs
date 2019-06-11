using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.Geth;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Nethereum.RPC.Eth.DTOs;
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
            public UniqueAddressList ContractAddresses = new UniqueAddressList();

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
        
        [Fact(Skip = "Work In Progress - Requires a private geth chain")]
        public async Task Run()
        {
            var contractAddresses = new[]
            {"0xd2e474c616cc60fb95d8b5f86c1043fa4552611b" };

            const ulong FromBlock = 4347;
            const ulong ToBlock = 4347;

            var web3geth = new Web3Geth();
            var transactionHandler = new SimpleTransactionHandler();
            var vmStackHandler = new VmStackHandler();
            var contractHandler = new ContractHandler();

            foreach (var contractAddress in contractAddresses)
            {
                contractHandler.ContractAddresses.Add(contractAddress);
            }

            var handlers = new HandlerContainer{ ContractHandler = contractHandler, TransactionHandler = transactionHandler, TransactionVmStackHandler = vmStackHandler};

            var blockProcessor = BlockProcessorFactory.Create(
                web3geth, 
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
