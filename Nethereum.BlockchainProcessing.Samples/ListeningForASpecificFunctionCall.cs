using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Web3Abstractions;
using Nethereum.Configuration;
using Nethereum.Contracts;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
            private readonly string _functionName = ABITypedRegistry.GetFunctionABI<BuyApprenticeFunction>().Name;

            public Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction)
            {
                return Task.CompletedTask;
            }

            public Task HandleTransactionAsync(TransactionWithReceipt txnWithReceipt)
            {
                if (!txnWithReceipt.IsForFunction<BuyApprenticeFunction>()) return Task.CompletedTask;

                var dto = txnWithReceipt.Decode<BuyApprenticeFunction>();

                Print(dto);

                return Task.CompletedTask;
            }

            private void Print(BuyApprenticeFunction dto)
            {
                System.Console.WriteLine($"[FUNCTION]");
                System.Console.WriteLine($"\t{_functionName}");

                foreach (var prop in dto.GetType().GetProperties())
                {
                    System.Console.WriteLine($"\t\t[{prop.Name}:{prop.GetValue(dto) ?? "null"}]");
                }
            }
        }

        public async Task Run()
        {
            ApplicationLogging.LoggerFactory.AddConsole(includeScopes: true);

            var targetBlockchain = new BlockchainSourceConfiguration(
                blockchainUrl: "https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60",
                name: "rinkeby") {FromBlock = 3146684, ToBlock = 3146684};
            
            var web3Wrapper = new Web3Wrapper(targetBlockchain.BlockchainUrl);
            var handlers = new HandlerContainer{ TransactionHandler = new BuyApprenticeFunctionHandler()};

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
