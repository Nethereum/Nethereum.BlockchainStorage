using Moq;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests.Azure
{
    public class AzureEventSearchProcessorTests
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

            public static string Signature() => new TransferEvent().GetEventABI().Sha3Signature;
        }

        [Fact]
        public async Task IsLogForEvent()
        {
            var indexer = new Mock<IEventSearchIndexer<TransferEvent>>();
            var processor = new EventLogSearchIndexProcessor<TransferEvent>(indexer.Object);

            var transferLog = new FilterLog {Topics = new object[]{ TransferEvent.Signature() }};
            var irrelevantLog = new FilterLog {Topics = new object[]{ "SignatureForAnotherEvent" }};

            Assert.True(processor.IsLogForEvent(transferLog));
            Assert.False(processor.IsLogForEvent(irrelevantLog));
        }

        [Fact]
        public async Task SendsToIndexerInBatchesAndSendsAnyPendingOnDispose()
        {
            var indexer = new Mock<IEventSearchIndexer<TransferEvent>>();
            var indexedLogs = new List<EventLog<TransferEvent>>();

            indexer.Setup(i => i.IndexAsync(It.IsAny<IEnumerable<EventLog<TransferEvent>>>()))
                .Returns<IEnumerable<EventLog<TransferEvent>>>(
                    logs => { indexedLogs.AddRange(logs); return Task.CompletedTask; });

            var processor = new EventLogSearchIndexProcessor<TransferEvent>(indexer.Object, logsPerIndexBatch: 10);

            var topics = new object[]
            {
                "0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef",
                "0x0000000000000000000000000000000000000000000000000000000000000000",
                "0x000000000000000000000000c14934679e71ef4d18b6ae927fe2b953c7fd9b91",
                "0x0000000000000000000000000000000000000000000000400000402000000001"
            };

            for (var i = 0; i < 25; i++)
            {
                var log = new  FilterLog {Topics = topics};

                await processor.ProcessLogsAsync(new[] {log});
            }

            Assert.Equal(20, indexedLogs.Count);
            Assert.Equal(5, processor.Pending);

            processor.Dispose();
            Assert.Equal(25, indexedLogs.Count);
            Assert.Equal(0, processor.Pending);
        }

        [Fact]
        public async Task AcceptsArraysOfEventLogsAndProcessesInBatches()
        {
            var indexer = new Mock<IEventSearchIndexer<TransferEvent>>();
            var indexedLogs = new List<EventLog<TransferEvent>>();

            indexer.Setup(i => i.IndexAsync(It.IsAny<IEnumerable<EventLog<TransferEvent>>>()))
                .Returns<IEnumerable<EventLog<TransferEvent>>>(
                    logs => { indexedLogs.AddRange(logs); return Task.CompletedTask; });

            var processor = new EventLogSearchIndexProcessor<TransferEvent>(indexer.Object, logsPerIndexBatch: 10);

            var topics = new object[]
            {
                "0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef",
                "0x0000000000000000000000000000000000000000000000000000000000000000",
                "0x000000000000000000000000c14934679e71ef4d18b6ae927fe2b953c7fd9b91",
                "0x0000000000000000000000000000000000000000000000400000402000000001"
            };

            var logsToSend = new FilterLog[25];

            for (var i = 0; i < 25; i++)
            {
                logsToSend[i] = new  FilterLog {Topics = topics};
            }

            await processor.ProcessLogsAsync(logsToSend);

            Assert.Equal(20, indexedLogs.Count);
            Assert.Equal(5, processor.Pending);

            processor.Dispose();
            Assert.Equal(25, indexedLogs.Count);
            Assert.Equal(0, processor.Pending);
        }

        [Fact]
        public async Task WillSwallowDecodingErrorsAndIgnoreLog()
        {
            var indexer = new Mock<IEventSearchIndexer<TransferEvent>>();
            var indexedLogs = new List<EventLog<TransferEvent>>();

            indexer.Setup(i => i.IndexAsync(It.IsAny<IEnumerable<EventLog<TransferEvent>>>()))
                .Returns<IEnumerable<EventLog<TransferEvent>>>(
                    logs => { indexedLogs.AddRange(logs); return Task.CompletedTask; });

            var processor = new EventLogSearchIndexProcessor<TransferEvent>(indexer.Object, logsPerIndexBatch: 10);

            var topicsForAnotherEvent = new object[]
            {
                "0xEdf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef",
                "0x0000000000000000000000000000000000000000000000000000000000000000",
                "0x000000000000000000000000c14934679e71ef4d18b6ae927fe2b953c7fd9b91",
                "0x0000000000000000000000000000000000000000000000400000402000000001"
            };

            await processor.ProcessLogsAsync(new[] {new  FilterLog {Topics = topicsForAnotherEvent}});

            Assert.Empty(indexedLogs);
            Assert.Equal(0, processor.Pending);

        }

    }
}
