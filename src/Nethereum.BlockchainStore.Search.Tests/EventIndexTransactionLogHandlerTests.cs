using Moq;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.BlockProcessing.ValueObjects;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests
{
    public class EventIndexTransactionLogHandlerTests
    {
        [Event("Transfer")]
        public class TransferEvent : IEventDTO
        {
            [Parameter("address", "_from", 1, true)]
            public string From {get; set;}

            [Parameter("address", "_to", 2, true)]
            public string To {get; set;}

            [Parameter("uint256", "_value", 3, true)]
            public BigInteger Value {get; set;}
        }

        [Fact]
        public async Task SendsToIndexInBatches()
        {
            var indexer = new Mock<IEventIndexer<TransferEvent>>();
            var indexedLogs = CaptureCallsToIndexer(indexer);

            var handler = new EventIndexTransactionLogHandler<TransferEvent>(indexer.Object, logsPerIndexBatch: 2);

            var transactionLog1 = new Mock<LogWithReceiptAndTransaction>();
            var transactionLog2 = new Mock<LogWithReceiptAndTransaction>();

            transactionLog1.Setup(t => t.IsForEvent<TransferEvent>()).Returns(true);
            transactionLog2.Setup(t => t.IsForEvent<TransferEvent>()).Returns(true);

            var eventLog1 = new EventLog<TransferEvent>(new TransferEvent(), new FilterLog());
            var eventLog2 = new EventLog<TransferEvent>(new TransferEvent(), new FilterLog());

            transactionLog1.Setup(t => t.Decode<TransferEvent>()).Returns(eventLog1);
            transactionLog2.Setup(t => t.Decode<TransferEvent>()).Returns(eventLog2);


            await handler.HandleAsync(transactionLog1.Object);
            Assert.Empty(indexedLogs);
            Assert.Equal(1, handler.Pending);
            await handler.HandleAsync(transactionLog2.Object);
            Assert.Equal(2, indexedLogs.Count);
            Assert.Equal(0, handler.Pending);
        }

        [Fact]
        public async Task WhenDisposeIsCalledWillAttemptToIndexPendingItems()
        {
            var indexer = new Mock<IEventIndexer<TransferEvent>>();
            var indexedLogs = CaptureCallsToIndexer(indexer);

            using (var handler =
                new EventIndexTransactionLogHandler<TransferEvent>(indexer.Object, logsPerIndexBatch: 2))
            {

                var transactionLog1 = new Mock<LogWithReceiptAndTransaction>();
                transactionLog1.Setup(t => t.IsForEvent<TransferEvent>()).Returns(true);

                var eventLog1 = new EventLog<TransferEvent>(new TransferEvent(), new FilterLog());
                transactionLog1.Setup(t => t.Decode<TransferEvent>()).Returns(eventLog1);

                await handler.HandleAsync(transactionLog1.Object);
                Assert.Equal(1, handler.Pending);
            }

            //calling dispose should cause app to process remaining pending items
            Assert.Single(indexedLogs);
        }

        [Fact]
        public async Task WillIgnoreLogsThatDoNotMatchTheEvent()
        {
            var indexer = new Mock<IEventIndexer<TransferEvent>>();
            var indexedLogs = CaptureCallsToIndexer(indexer);
            var handler =
                new EventIndexTransactionLogHandler<TransferEvent>(indexer.Object, logsPerIndexBatch: 1);

            var logForAnotherEvent = new Mock<LogWithReceiptAndTransaction>();
            logForAnotherEvent.Setup(t => t.IsForEvent<TransferEvent>()).Returns(false);

            await handler.HandleAsync(logForAnotherEvent.Object);
            Assert.Empty(indexedLogs);
            Assert.Equal(0, handler.Pending);
            
        }

        private List<EventLog<TransferEvent>> CaptureCallsToIndexer(Mock<IEventIndexer<TransferEvent>> indexer)
        {
            var indexedLogs = new List<EventLog<TransferEvent>>();

            indexer
                .Setup(i => i.IndexAsync(It.IsAny<IEnumerable<EventLog<TransferEvent>>>()))
                .Returns<IEnumerable<EventLog<TransferEvent>>>(l =>
                {
                    indexedLogs.AddRange(l);
                    return Task.CompletedTask;
                });

            return indexedLogs;
        }
    }
}
