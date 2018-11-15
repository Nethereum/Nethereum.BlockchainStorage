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
    public class ListeningForASpecificEvent
    {
        /*
 Solidity Contract Excerpt
    * event Transfer(address indexed _from, address indexed _to, uint256 indexed _value);
 Other contracts may have transfer events with different signatures, this won't work for those.
*/
        [Event("Transfer")]
        public class TransferEvent
        {
            [Parameter("address", "_from", 1, true)]
            public string From {get; set;}

            [Parameter("address", "_to", 2, true)]
            public string To {get; set;}

            [Parameter("uint256", "_value", 3, true)]
            public BigInteger Value {get; set;}
        }

        public class TransferEventHandler: ITransactionLogHandler<TransferEvent>
        {
            private readonly string _eventName = ABITypedRegistry.GetEvent<TransferEvent>().Name;

            public Task HandleAsync(TransactionLogWrapper transactionLog)
            {
                try
                {
                    if (!transactionLog.IsForEvent<TransferEvent>()) return Task.CompletedTask;

                    Console.WriteLine($"Tx Hash:{transactionLog.Transaction.TransactionHash}, LogIndex: {transactionLog.LogIndex}");

                    var eventValues = transactionLog.Decode<TransferEvent>();
                    if (eventValues == null) return Task.CompletedTask;

                    PrintEvent(eventValues);
                }
                catch (Exception x)
                {
                    Console.WriteLine("Error whilst handling transaction log - expected event signature may differ from the expected event.");
                    Console.WriteLine(x.Message);
                }

                return Task.CompletedTask;
            }

            private void PrintEvent(EventLog<TransferEvent> eventValues)
            {
                System.Console.WriteLine($"[EVENT]");
                System.Console.WriteLine($"\t[{_eventName}]");
                foreach (var prop in eventValues?.Event.GetType().GetProperties())
                {
                    System.Console.WriteLine($"\t\t[{prop.Name}:{prop.GetValue(eventValues.Event) ?? "null"}]");
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
            var handlers = new HandlerContainer{ TransactionLogHandler = new TransferEventHandler()};

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
