using Moq;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.LogProcessing.Tests.Processors
{
    public class EventLogQueueProcessorTests
    {
        [Fact]
        public void IsLogForEvent_WhenPredicateIsNull_ReturnsTrue()
        {
            var queue = new Mock<IQueue>();
            var processor = new EventLogQueueProcessor(queue.Object);
            var transferEvent = TestData.Contracts.StandardContract.SampleTransferLog();
            Assert.True(processor.IsLogForEvent(transferEvent));
        }

        [Fact]
        public void IsLogForEvent_WhenPredicateIsNotNull_ReturnsPredicateResult()
        {
            var queue = new Mock<IQueue>();
            var processor = new EventLogQueueProcessor(queue.Object, predicate: log => false);
            var transferEvent = TestData.Contracts.StandardContract.SampleTransferLog();
            Assert.False(processor.IsLogForEvent(transferEvent));
        }

        [Fact]
        public async Task Process_AddsLogsToQueue()
        {
            var queue = new Mock<IQueue>();
            var messages = new List<object>();
            queue
                .Setup(q => q.AddMessageAsync(It.IsAny<object>()))
                .Callback<object>(o => messages.Add(o)).Returns(Task.CompletedTask);

            var processor = new EventLogQueueProcessor(queue.Object);

            var logs = new[]{
                TestData.Contracts.StandardContract.SampleTransferLog(),
                TestData.Contracts.StandardContract.SampleTransferLog()};

            await processor.ProcessLogsAsync(logs);

            Assert.Equal(logs.Length, messages.Count);
            foreach (var message in messages)
            {
                Assert.IsType<FilterLog>(message);
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

            var processor = new EventLogQueueProcessor(
                queue.Object,
                mapper: log => new QueueMessage { TransactionHash = log.TransactionHash, LogIndex = log.LogIndex });

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

            var processor = new EventLogQueueProcessor(
                queue.Object, predicate: tfr => false);

            var logs = new[] { TestData.Contracts.StandardContract.SampleTransferLog() };

            await processor.ProcessLogsAsync(logs);

            Assert.Empty(queudMessages);
        }
    }
}
