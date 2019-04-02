using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Linq;
using System.Numerics;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs
{
    public class DecodedEventTests
    {
        [Fact]
        public void ToDecodedEventWithNullAbi()
        {
            var log = new FilterLog{};
            var decodedEvent = log.ToDecodedEvent();

            Assert.NotNull(decodedEvent);
            Assert.Empty(decodedEvent.Event);
            Assert.Same(log, decodedEvent.Log);

            Assert.Null(decodedEvent.State["EventAbiName"]);
            Assert.Null(decodedEvent.State["EventSignature"]);
            Assert.Null(decodedEvent.State["TransactionHash"]);
            Assert.Null(decodedEvent.State["LogIndex"]);
            Assert.Equal(0, decodedEvent.State["HandlerInvocations"]);
            Assert.True(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >= ((long) decodedEvent.State["UtcNowMs"]));
        }

        [Fact]
        public void ToDecodedEventWithAbi()
        {
            var log = TestData.Contracts.StandardContract.SampleTransferLog();
            var decodedEvent = log.ToDecodedEvent(TestData.Contracts.StandardContract.TransferEventAbi);

            Assert.NotNull(decodedEvent);
            Assert.Same(log, decodedEvent.Log);

            Assert.Equal("Transfer", decodedEvent.State["EventAbiName"]);
            Assert.Equal(TestData.Contracts.StandardContract.TransferEventSignature, decodedEvent.State["EventSignature"]);
            Assert.Equal("0x19ce02e0b4fdf5cfee0ed21141b38c2d88113c58828c771e813ce2624af127cd", decodedEvent.State["TransactionHash"]);
            Assert.Equal((BigInteger)0, ((BigInteger)decodedEvent.State["LogIndex"]));
            Assert.Equal(0, decodedEvent.State["HandlerInvocations"]);
            Assert.True(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >= ((long) decodedEvent.State["UtcNowMs"]));

            Assert.Equal(3,decodedEvent.Event.Count);

            var param1 = decodedEvent.Event.FirstOrDefault(p => p.Parameter.Order == 1);
            Assert.Equal("0x12890d2cce102216644c59dae5baed380d84830c", param1.Result);
            var param2 = decodedEvent.Event.FirstOrDefault(p => p.Parameter.Order == 2);
            Assert.Equal("0x13f022d72158410433cbd66f5dd8bf6d2d129924", param2.Result);
            var param3 = decodedEvent.Event.FirstOrDefault(p => p.Parameter.Order == 3);
            Assert.Equal(new BigInteger(1000), param3.Result);
        }
    }
}
