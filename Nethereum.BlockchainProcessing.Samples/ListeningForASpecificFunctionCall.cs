using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.Contracts;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Xunit;

namespace Nethereum.BlockchainProcessing.Samples
{
    public class ListeningForASpecificFunctionCall
    {
        [Function("buyApprenticeChest")]
        public class BuyApprenticeFunction: FunctionMessage
        {
            [Parameter("uint256", "_region", 1)]
            public BigInteger Region { get; set; }
        }

        public class BuyApprenticeFunctionHandler: ITransactionHandler<BuyApprenticeFunction>
        {
            public List<(TransactionWithReceipt, BuyApprenticeFunction)> FunctionsHandled = new List<(TransactionWithReceipt, BuyApprenticeFunction)>();

            public Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction)
            {
                return Task.CompletedTask;
            }

            public Task HandleTransactionAsync(TransactionWithReceipt txnWithReceipt)
            {
                //we are not using conditional TransactionRouting in this example (see TransactionRouter)
                //therefore we need this check to avoid handling non related transactions
                if (!txnWithReceipt.IsForFunction<BuyApprenticeFunction>()) return Task.CompletedTask;

                var dto = txnWithReceipt.Decode<BuyApprenticeFunction>();
                FunctionsHandled.Add((txnWithReceipt, dto));
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task Run()
        {            
            var blockchainProxyService = new BlockchainProxyService(TestConfiguration.BlockchainUrls.Infura.Rinkeby);
            var buyApprenticeFunctionHandler = new BuyApprenticeFunctionHandler();
            var handlers = new HandlerContainer{ TransactionHandler = buyApprenticeFunctionHandler};

            var blockProcessor = BlockProcessorFactory.Create(
                blockchainProxyService, 
                handlers,
                processTransactionsInParallel: false);

            var processingStrategy = new ProcessingStrategy(blockProcessor);
            var blockchainProcessor = new BlockchainProcessor(processingStrategy);

            var result = await blockchainProcessor.ExecuteAsync(3146684, 3146684);

            Assert.True(result);
            Assert.Single(buyApprenticeFunctionHandler.FunctionsHandled);
        }
    }
}
