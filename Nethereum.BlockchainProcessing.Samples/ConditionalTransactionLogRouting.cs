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
    public class ConditionalTransactionLogRouting
    {
        #region DTO's to represent event parameters for a known event on a known solidity contract
        /*
         * Solidity Excerpt
         * event Transfer(address indexed _from, address indexed _to, uint256 indexed _value);
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
        #endregion

        public class CatchAllEventLogHandler : ITransactionLogHandler
        {
            public List<TransactionLogWrapper> EventsHandled = new List<TransactionLogWrapper>();

            public Task HandleAsync(TransactionLogWrapper transactionLog)
            {
                EventsHandled.Add(transactionLog);
                return Task.CompletedTask;
            }
        }

        public class SpecificEventLogHandler<TEvent> : ITransactionLogHandler<TEvent> where TEvent : new()
        {
            public List<(TransactionLogWrapper, EventLog<TransferEvent>)> EventsHandled = 
                new List<(TransactionLogWrapper, EventLog<TransferEvent>)>();

            public Task HandleAsync(TransactionLogWrapper transactionLog)
            {
                var eventValues = transactionLog.Decode<TransferEvent>();
                EventsHandled.Add((transactionLog, eventValues));
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task Run()
        {
            var blockchainProxyService = new BlockchainProxyService(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            var catchAllHandler = new CatchAllEventLogHandler();
            var transferHandler = new SpecificEventLogHandler<TransferEvent>();

            var transactionLogRouter = new TransactionLogRouter();

            //to be invoked for every tx
            transactionLogRouter.AddHandler(catchAllHandler);

            //to be invoked if tx is from a specific address and event matches
            transactionLogRouter.AddHandler(
                (txn) => txn.Transaction.IsTo("0xC03cDD393C89D169bd4877d58f0554f320f21037"), 
                transferHandler);

            var handlers = new HandlerContainer{ TransactionLogHandler = transactionLogRouter};

            var blockProcessor = BlockProcessorFactory.Create(
                blockchainProxyService, 
                handlers,
                processTransactionsInParallel: false);

            var processingStrategy = new ProcessingStrategy(blockProcessor);
            var blockchainProcessor = new BlockchainProcessor(processingStrategy);

            var result = await blockchainProcessor.ExecuteAsync(3146684, 3146684);

            Assert.True(result);
            Assert.Single(transferHandler.EventsHandled);
            Assert.Equal(7, catchAllHandler.EventsHandled.Count);

        }
    }
}
