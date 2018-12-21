using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.Model;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Xunit;

namespace Nethereum.BlockchainProcessing.Samples
{
    public class ConditionalTransactionRouting
    {
        #region DTO's to represent function input parameters for a known solidity contract
        [Function("buyApprenticeChest")]
        public class BuyApprenticeFunction: FunctionMessage
        {
            [Parameter("uint256", "_region", 1)]
            public BigInteger Region { get; set; }
        }

        [Function("openChest")]
        public class OpenChestFunction: FunctionMessage
        {
            [Parameter("uint256", "_identifier", 1)]
            public BigInteger Identifier { get; set; }
        }
        #endregion

        public class CatchAllTransactionHandler : ITransactionHandler
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

        public class FunctionHandler<TFunctionInput>: ITransactionHandler<TFunctionInput> where TFunctionInput : FunctionMessage, new()
        {
            public List<ContractCreationTransaction> ContractCreationTransactionsHandled = new List<ContractCreationTransaction>();
            public List<(TransactionWithReceipt, TFunctionInput)> FunctionsHandled = new List<(TransactionWithReceipt, TFunctionInput)>();

            public Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction)
            {
                ContractCreationTransactionsHandled.Add(contractCreationTransaction);
                return Task.CompletedTask;
            }

            public Task HandleTransactionAsync(TransactionWithReceipt txnWithReceipt)
            {
                var dto = txnWithReceipt.Decode<TFunctionInput>();
                FunctionsHandled.Add((txnWithReceipt, dto));
                return Task.CompletedTask;
            }

        }

        [Fact]
        public async Task Run()
        {
            var blockchainProxyService = new BlockchainProxyService("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");

            var catchAllHandler = new CatchAllTransactionHandler();
            var openChestHandler = new FunctionHandler<OpenChestFunction>();
            var buyApprenticeHandler = new FunctionHandler<BuyApprenticeFunction>();

            var transactionRouter = new TransactionRouter();

            //to be invoked for every tx
            transactionRouter.AddTransactionHandler(catchAllHandler);

            //to be invoked if function matches
            transactionRouter.AddTransactionHandler(openChestHandler);

            //to be invoked if tx is from a specific address and function matches
            transactionRouter.AddTransactionHandler(
                (txn) => txn.Transaction.IsTo("0xC03cDD393C89D169bd4877d58f0554f320f21037"), 
                buyApprenticeHandler);

            var handlers = new HandlerContainer{ TransactionHandler = transactionRouter};

            var blockProcessor = BlockProcessorFactory.Create(
                blockchainProxyService, 
                handlers,
                processTransactionsInParallel: false);

            var processingStrategy = new ProcessingStrategy(blockProcessor);
            var blockchainProcessor = new BlockchainProcessor(processingStrategy);

            //run once to catch first instance of function call
            var result = await blockchainProcessor.ExecuteAsync(3146684, 3146684);
            //run again (deliberately skipping irrelevant blocks to reduce unit test duration)
            result = await blockchainProcessor.ExecuteAsync(3146709, 3146709);

            Assert.True(result);
            Assert.Single(openChestHandler.FunctionsHandled);
            Assert.Empty(openChestHandler.ContractCreationTransactionsHandled);
            Assert.Single(buyApprenticeHandler.FunctionsHandled);
            Assert.Empty(buyApprenticeHandler.ContractCreationTransactionsHandled);
            Assert.Equal(31, catchAllHandler.TransactionsHandled.Count);

        }
    }
}
