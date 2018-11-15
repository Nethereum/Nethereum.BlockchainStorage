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
using Nethereum.ABI.Model;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Samples
{
    public class ConditionalTransactionRouting
    {
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

        public class BasicTransactionPrinter : ITransactionHandler
        {
            public Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction)
            {
                System.Console.WriteLine($"Hash: {contractCreationTransaction.Transaction.TransactionHash}, Sender:{contractCreationTransaction.Transaction.From}");
                return Task.CompletedTask;
            }

            public Task HandleTransactionAsync(TransactionWithReceipt transactionWithReceipt)
            {
                System.Console.WriteLine($"Hash: {transactionWithReceipt.Transaction.TransactionHash}, Sender:{transactionWithReceipt.Transaction.From}");
                return Task.CompletedTask;
            }
        }

        public class FunctionPrinter<TFunctionInput>: ITransactionHandler<TFunctionInput> where TFunctionInput : FunctionMessage, new()
        {
            private readonly FunctionABI _functionAbi = ABITypedRegistry.GetFunctionABI<TFunctionInput>();

            public Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction)
            {
                return Task.CompletedTask;
            }

            public Task HandleTransactionAsync(TransactionWithReceipt txnWithReceipt)
            {
                if (!txnWithReceipt.IsForFunction<TFunctionInput>())
                    return Task.CompletedTask;

                var dto = txnWithReceipt.Decode<TFunctionInput>();

                Print(dto);

                return Task.CompletedTask;
            }

            private void Print(TFunctionInput dto)
            {
                System.Console.WriteLine($"[FUNCTION]");
                System.Console.WriteLine($"\t{_functionAbi.Name ?? "unknown"}");

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
                name: "rinkeby") {FromBlock = 3146684, ToBlock = 3146709};
            
            var web3Wrapper = new Web3Wrapper(targetBlockchain.BlockchainUrl);

            var transactionRouter = new TransactionRouter();

            //to be invoked for every tx
            transactionRouter.AddTransactionHandler(new BasicTransactionPrinter());

            //to be invoked if function matches
            transactionRouter.AddTransactionHandler<OpenChestFunction>(
                new FunctionPrinter<OpenChestFunction>());

            //to be invoked if tx is from a specific address and function matches
            transactionRouter.AddTransactionHandler<BuyApprenticeFunction>(
                (txn) => txn.Transaction.IsTo("0xC03cDD393C89D169bd4877d58f0554f320f21037"), 
                new FunctionPrinter<BuyApprenticeFunction>());

            var handlers = new HandlerContainer{ TransactionHandler = transactionRouter};

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
