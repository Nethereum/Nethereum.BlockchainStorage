using Moq;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Web3Abstractions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs
{
    public class BlockchainLogProcessorTests
    {
        [Fact]
        public async Task Allocates_Matching_Logs_To_Processors()
        {
            var eventLogProxy = new Mock<IEventLogProxy>();
            var log1Processor = new Mock<ILogProcessor>();
            var log2Processor = new Mock<ILogProcessor>();
            var catchAllProcessor = new Mock<ILogProcessor>();

            var mockProcessors = new[] {log1Processor, log2Processor, catchAllProcessor };

            var logProcessor = new BlockchainLogProcessor(
                eventLogProxy.Object, mockProcessors.Select(p => p.Object));

            var log1 = new FilterLog();
            var log2 = new FilterLog();
            var log3 = new FilterLog();

            var logs = new []{ log1, log2, log3};

            log1Processor.Setup(p => p.IsLogForEvent(log1)).Returns(true);
            log2Processor.Setup(p => p.IsLogForEvent(log2)).Returns(true);

            catchAllProcessor
                .Setup(p => p.IsLogForEvent(It.IsAny<FilterLog>()))
                .Returns(true);

            eventLogProxy
                .Setup(p => p.GetLogs(It.IsAny<NewFilterInput>(), null))
                .ReturnsAsync(logs);

            var processedLogs = new Dictionary<Mock<ILogProcessor>, List<FilterLog>>();
            foreach (var processor in mockProcessors)
            {
                processedLogs.Add(processor, new List<FilterLog>());

                processor.Setup(p => p.ProcessLogsAsync(It.IsAny<FilterLog[]>()))
                    .Callback<FilterLog[]>(l => processedLogs[processor].AddRange(l))
                    .Returns(Task.CompletedTask);
            }

            await logProcessor.ProcessAsync(new BlockRange(0, 0));

            Assert.Single(processedLogs[log1Processor], log1);
            Assert.Single(processedLogs[log2Processor], log2);
            Assert.True(logs.SequenceEqual(processedLogs[catchAllProcessor]));
        }

        [Fact]
        public async Task Dedupes_Logs_Matching_Multiple_Filters()
        {
            var eventLogProxy = new Mock<IEventLogProxy>();
            var catchAllProcessor = new Mock<ILogProcessor>();

            var filter1 = new NewFilterInput();
            var filter2 = new NewFilterInput();
            var filters = new[] {filter1, filter2};

            var mockProcessors = new[] {catchAllProcessor };

            var logProcessor = new BlockchainLogProcessor(
                eventLogProxy.Object, mockProcessors.Select(p => p.Object), filters);

            var log1 = new FilterLog(){TransactionHash = "x", LogIndex = new HexBigInteger(0)};
            var duplicateLog = new FilterLog(){TransactionHash = "x", LogIndex = new HexBigInteger(0)};
            var log2 = new FilterLog(){TransactionHash = "y", LogIndex = new HexBigInteger(0)};

            var logsFromFilter1 = new []{log1, duplicateLog};
            var logsFromFilter2 = new []{log2, duplicateLog};

            catchAllProcessor
                .Setup(p => p.IsLogForEvent(It.IsAny<FilterLog>()))
                .Returns(true);

            eventLogProxy
                .Setup(p => p.GetLogs(filter1, null))
                .ReturnsAsync(logsFromFilter1);

            eventLogProxy
                .Setup(p => p.GetLogs(filter2, null))
                .ReturnsAsync(logsFromFilter2);

            var processedLogs = new List<FilterLog>();
            catchAllProcessor.Setup(p => p.ProcessLogsAsync(It.IsAny<FilterLog[]>()))
                .Callback<FilterLog[]>(l => processedLogs.AddRange(l))
                .Returns(Task.CompletedTask);

            await logProcessor.ProcessAsync(new BlockRange(0, 0));

            Assert.Equal(2, processedLogs.Count);
            Assert.Contains(log1, processedLogs);
            Assert.Contains(log2, processedLogs);
            Assert.DoesNotContain(duplicateLog, processedLogs);
        }

        [Fact]
        public async Task Retrieves_Logs_For_Each_Filter()
        {
            var eventLogProxy = new Mock<IEventLogProxy>();
            var catchAllProcessor = new Mock<ILogProcessor>();

            var filter1 = new NewFilterInput();
            var filter2 = new NewFilterInput();
            var filters = new[] {filter1, filter2};

            var mockProcessors = new[] {catchAllProcessor };

            var logProcessor = new BlockchainLogProcessor(
                eventLogProxy.Object, mockProcessors.Select(p => p.Object), filters);

            var log1 = new FilterLog(){TransactionHash = "x", LogIndex = new HexBigInteger(0)};
            var log2 = new FilterLog(){TransactionHash = "y", LogIndex = new HexBigInteger(0)};

            catchAllProcessor
                .Setup(p => p.IsLogForEvent(It.IsAny<FilterLog>()))
                .Returns(true);

            eventLogProxy
                .Setup(p => p.GetLogs(filter1, null))
                .ReturnsAsync(new []{log1});

            eventLogProxy
                .Setup(p => p.GetLogs(filter2, null))
                .ReturnsAsync(new []{log2});

            var processedLogs = new List<FilterLog>();
            catchAllProcessor.Setup(p => p.ProcessLogsAsync(It.IsAny<FilterLog[]>()))
                .Callback<FilterLog[]>(l => processedLogs.AddRange(l))
                .Returns(Task.CompletedTask);

            await logProcessor.ProcessAsync(new BlockRange(0, 0));

            Assert.Equal(2, processedLogs.Count);
            Assert.Contains(log1, processedLogs);
            Assert.Contains(log2, processedLogs);
        }

        [Fact]
        public async Task Passes_Correct_Block_Range_To_Proxy()
        {
            var eventLogProxy = new Mock<IEventLogProxy>();

            var logProcessor = new BlockchainLogProcessor(
                eventLogProxy.Object, Array.Empty<ILogProcessor>());

            var logs = Array.Empty<FilterLog>();

            NewFilterInput actualFilter = null;

            eventLogProxy
                .Setup(p => p.GetLogs(It.IsAny<NewFilterInput>(), null))
                .Callback<NewFilterInput, object>((f,o) => actualFilter = f)
                .ReturnsAsync(logs);

            await logProcessor.ProcessAsync(new BlockRange(0, 10));

            Assert.NotNull(actualFilter);
            Assert.Equal(0, actualFilter.FromBlock.BlockNumber.Value);
            Assert.Equal(10, actualFilter.ToBlock.BlockNumber.Value);
        }

        [Fact]
        public async Task When_Cancellation_Is_Requested_Does_Not_Call_Processor()
        {
            var eventLogProxy = new Mock<IEventLogProxy>();
            var catchAllProcessor = new Mock<ILogProcessor>();

            var logProcessor = new BlockchainLogProcessor(
                eventLogProxy.Object, new []{catchAllProcessor.Object});

            catchAllProcessor
                .Setup(p => p.IsLogForEvent(It.IsAny<FilterLog>()))
                .Returns(true);

            var logs = new [] {new FilterLog()};

            var cancellationToken = new CancellationTokenSource();

            //fake cancellation being raised after logs are retrieved but before processing
            eventLogProxy
                .Setup(p => p.GetLogs(It.IsAny<NewFilterInput>(), null))
                .Callback<NewFilterInput, object>((f, o) => cancellationToken.Cancel())
                .ReturnsAsync(logs);

            await logProcessor.ProcessAsync(new BlockRange(0, 10), cancellationToken.Token);

            catchAllProcessor
                .Verify(p => p.IsLogForEvent(It.IsAny<FilterLog>()), Times.Never);
        }

        [Fact]
        public async Task Checks_For_Cancellation_Before_Each_Processing_Batch()
        {
            var eventLogProxy = new Mock<IEventLogProxy>();
            var catchAllProcessor1 = new Mock<ILogProcessor>();
            var catchAllProcessor2 = new Mock<ILogProcessor>();

            catchAllProcessor1
                .Setup(p => p.IsLogForEvent(It.IsAny<FilterLog>()))
                .Returns(true);

            catchAllProcessor2
                .Setup(p => p.IsLogForEvent(It.IsAny<FilterLog>()))
                .Returns(true);

            var logProcessor = new BlockchainLogProcessor(
                eventLogProxy.Object, new []{catchAllProcessor1.Object, catchAllProcessor2.Object});

            var logs = new [] {new FilterLog()};

            var cancellationToken = new CancellationTokenSource();

            eventLogProxy
                .Setup(p => p.GetLogs(It.IsAny<NewFilterInput>(), null))
                .ReturnsAsync(logs);

            //cancel after processor 1 finishes
            catchAllProcessor1
                .Setup(p => p.ProcessLogsAsync(It.IsAny<FilterLog[]>()))
                .Callback<FilterLog[]>(l => cancellationToken.Cancel())
                .Returns(Task.CompletedTask);

            await logProcessor.ProcessAsync(new BlockRange(0, 10), cancellationToken.Token);

            catchAllProcessor1
                .Verify(p => p.ProcessLogsAsync(It.IsAny<FilterLog[]>()), Times.Once);

            catchAllProcessor2
                .Verify(p => p.ProcessLogsAsync(It.IsAny<FilterLog[]>()), Times.Never);
        }
    }
}
