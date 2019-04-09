using Moq;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Matching;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs
{
    public class EventParameterMatcherTests
    {
        EventABI[] _eventAbis = new []{ TestData.Contracts.StandardContract.TransferEventAbi };
        FilterLog _log = new FilterLog();

        [Fact]
        public void WhenThereAreNoConditionsReturnsTrue()
        {
            var matcher = new EventParameterMatcher(Array.Empty<IParameterCondition>());
            Assert.True(matcher.IsMatch(_eventAbis, _log));
        }

        [Fact]
        public void WhenAConditionReturnsFalseItDoesNotMatch()
        {
            var mockCondition = new Mock<IParameterCondition>();
            mockCondition.Setup(c => c.IsTrue(It.IsAny<EventLog<List<ParameterOutput>>>())).Returns(false);

            var matcher = new EventParameterMatcher(new IParameterCondition[]{ mockCondition.Object});
            Assert.False(matcher.IsMatch(_eventAbis, _log));
        }

        [Fact]
        public void AllConditionsMustReturnTrueToMatch()
        {
            var doNotMatch = new Mock<IParameterCondition>();
            doNotMatch.Setup(c => c.IsTrue(It.IsAny<EventLog<List<ParameterOutput>>>())).Returns(false);

            var match = new Mock<IParameterCondition>();
            match.Setup(c => c.IsTrue(It.IsAny<EventLog<List<ParameterOutput>>>())).Returns(true);

            var matcher = new EventParameterMatcher(new IParameterCondition[]{ doNotMatch.Object, match.Object});
            Assert.False(matcher.IsMatch(_eventAbis, _log));
        }

        [Fact]
        public void WhenAllConditionsReturnTrueItDoesMatch()
        {
            var mockCondition1 = new Mock<IParameterCondition>();
            mockCondition1.Setup(c => c.IsTrue(It.IsAny<EventLog<List<ParameterOutput>>>())).Returns(true);
            var mockCondition2 = new Mock<IParameterCondition>();
            mockCondition2.Setup(c => c.IsTrue(It.IsAny<EventLog<List<ParameterOutput>>>())).Returns(true);

            var matcher = new EventParameterMatcher(new IParameterCondition[]{ mockCondition1.Object, mockCondition2.Object});
            Assert.True(matcher.IsMatch(_eventAbis, _log));
        }

        [Fact]
        public void DecodesEventBeforeParameterEvaluation()
        {
            EventLog<List<ParameterOutput>> actualDecodedParameters = null;

            var mockCondition1 = new Mock<IParameterCondition>();
            mockCondition1
                .Setup(c => c.IsTrue(It.IsAny<EventLog<List<ParameterOutput>>>()))
                .Returns<EventLog<List<ParameterOutput>>>((decodedParameters) => { 
                    actualDecodedParameters = decodedParameters;
                    return true;
                });

            var matcher = new EventParameterMatcher(new IParameterCondition[]{ mockCondition1.Object});
            matcher.IsMatch(_eventAbis, TestData.Contracts.StandardContract.SampleTransferLog());

            Assert.NotNull(actualDecodedParameters);
            Assert.Equal("0x19ce02e0b4fdf5cfee0ed21141b38c2d88113c58828c771e813ce2624af127cd", actualDecodedParameters.Log.TransactionHash);
            Assert.Equal("0x12890d2cce102216644c59dae5baed380d84830c", actualDecodedParameters.Event[0].Result);
        }
    }
}
