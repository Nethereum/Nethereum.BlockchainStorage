using Moq;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.LogProcessing.Tests.Processors
{
    public class EventLogQueueProcessorOfTTests
    {
        [Fact]
        public void IsLogForEvent_WhenTheEventMatchesReturnsTrue()
        {
            var queue = new Mock<IQueue>();

            var processor = new EventLogQueueProcessor<TestData.Contracts.StandardContract.TransferEvent>(queue.Object);

            var transferEvent = TestData.Contracts.StandardContract.SampleTransferLog();
            var nonTransferLog = new FilterLog();

            Assert.True(processor.IsLogForEvent(transferEvent));
            Assert.False(processor.IsLogForEvent(nonTransferLog));
        }

        [Fact]
        public async Task Process_DecodesLogAndAddsToQueue()
        {
            var queue = new Mock<IQueue>();
            var messages = new List<object>();
            queue
                .Setup(q => q.AddMessageAsync(It.IsAny<object>()))
                .Callback<object>(o => messages.Add(o)).Returns(Task.CompletedTask);

            var processor = new EventLogQueueProcessor<TestData.Contracts.StandardContract.TransferEvent>(queue.Object);

            var logs = new[]{
                TestData.Contracts.StandardContract.SampleTransferLog(),
                TestData.Contracts.StandardContract.SampleTransferLog()};

            await processor.ProcessLogsAsync(logs);

            Assert.Equal(logs.Length, messages.Count);
            foreach (var message in messages)
            {
                Assert.IsType<EventLog<TestData.Contracts.StandardContract.TransferEvent>>(message);
            }
        }

        public class QueueMessage
        {
            public string TransactionHash { get; set; }
            public HexBigInteger LogIndex { get; set; }
        }

        [Fact]
        public async Task Process_WhenMapperIsNotNull_WritesMappedLogsToQueue()
        {
            var queue = new Mock<IQueue>();
            var queudMessages = new List<object>();
            queue
                .Setup(q => q.AddMessageAsync(It.IsAny<object>()))
                .Callback<object>(o => queudMessages.Add(o)).Returns(Task.CompletedTask);

            var processor = new EventLogQueueProcessor<TestData.Contracts.StandardContract.TransferEvent>(
                queue.Object,
                mapper: tfr => new QueueMessage { TransactionHash = tfr.Log.TransactionHash, LogIndex = tfr.Log.LogIndex });

            var logs = new[]{
                TestData.Contracts.StandardContract.SampleTransferLog(),
                TestData.Contracts.StandardContract.SampleTransferLog()};

            await processor.ProcessLogsAsync(logs);

            Assert.Equal(logs.Length, queudMessages.Count);
            foreach (var message in queudMessages)
            {
                Assert.IsType<QueueMessage>(message);
            }
        }

        [Fact]
        public async Task Process_WhenPredicateReturnsFalseDoesNotAddToQueue()
        {
            var queue = new Mock<IQueue>();
            var queudMessages = new List<object>();
            queue
                .Setup(q => q.AddMessageAsync(It.IsAny<object>()))
                .Callback<object>(o => queudMessages.Add(o)).Returns(Task.CompletedTask);

            var processor = new EventLogQueueProcessor<TestData.Contracts.StandardContract.TransferEvent>(
                queue.Object, predicate: tfr => false);

            var logs = new[] { TestData.Contracts.StandardContract.SampleTransferLog() };

            await processor.ProcessLogsAsync(logs);

            Assert.Empty(queudMessages);
        }
    }
}
