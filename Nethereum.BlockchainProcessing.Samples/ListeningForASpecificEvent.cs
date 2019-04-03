using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Xunit;

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
            public List<(TransactionLogWrapper, EventLog<TransferEvent>)> TransferEventsHandled = 
                new List<(TransactionLogWrapper, EventLog<TransferEvent>)>();

            public List<TransactionLogWrapper> TransferEventsWithDifferentSignature = 
                new List<TransactionLogWrapper>();

            public Task HandleAsync(TransactionLogWrapper transactionLog)
            {
                try
                {
                    if(!transactionLog.IsForEvent<TransferEvent>()) return Task.CompletedTask;

                    var eventValues = transactionLog.Decode<TransferEvent>();
                    TransferEventsHandled.Add((transactionLog, eventValues));
                }
                catch (Exception)
                {
                    //Error whilst handling transaction log
                    //expected event signature may differ from the expected event.
                    TransferEventsWithDifferentSignature.Add(transactionLog);
                }

                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task Run()
        {            
            var blockchainProxyService = new BlockchainProxyService(TestConfiguration.BlockchainUrls.Infura.Rinkeby);
            var transferEventHandler = new TransferEventHandler();
            var handlers = new HandlerContainer{ TransactionLogHandler = transferEventHandler};

            var blockProcessor = BlockProcessorFactory.Create(
                blockchainProxyService, 
                handlers,
                processTransactionsInParallel: false);

            var processingStrategy = new ProcessingStrategy(blockProcessor);
            var blockchainProcessor = new BlockchainProcessor(processingStrategy);

            var result = await blockchainProcessor.ExecuteAsync(3146684, 3146684);

            Assert.True(result);

            //this is our expected event (see TransferEvent class)
            Assert.Single(transferEventHandler.TransferEventsHandled);

            //there is an event from another contract called Transfer
            //it can't be deserialized because
            //the number of indexed fields is different
            Assert.Single(transferEventHandler.TransferEventsWithDifferentSignature);
        }
    }
}
