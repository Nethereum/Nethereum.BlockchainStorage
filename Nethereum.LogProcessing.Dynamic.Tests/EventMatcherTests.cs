using Moq;
using Nethereum.ABI.Model;
using Nethereum.BlockchainProcessing.Processing.Logs.Matching;
using Nethereum.RPC.Eth.DTOs;
using Xunit;

namespace Nethereum.LogProcessing.Tests
{
    public class EventMatcherTests
    {
        FilterLog _log = TestData.Contracts.StandardContract.SampleTransferLog();
        EventABI[] _eventAbis = new []{ TestData.Contracts.StandardContract.TransferEventAbi };

        [Fact]
        public void GivenNoMatchersWillReturnTrue()
        {
            var matcher = new EventMatcher();
            Assert.True(matcher.IsMatch(_log));
        }

        [Fact]
        public void GivenOnlyTheEventAbi_MatchesEventSignature()
        {
            var matcher = new EventMatcher(_eventAbis);
            Assert.True(matcher.IsMatch(_log));
        }

        [Fact]
        public void GivenManyEventAbis_MatchesFirstEventSignature()
        {
            EventABI[] eventAbis = new[] { 
                TestData.Contracts.StandardContract.ApprovalEventAbi, 
                TestData.Contracts.StandardContract.TransferEventAbi };

            var matcher = new EventMatcher(eventAbis);
            Assert.True(matcher.IsMatch(_log));
        }

        [Fact]
        public void DoesNotMisMatchEventSignature()
        {
            EventABI[] eventAbis = new[] {
                TestData.Contracts.StandardContract.ApprovalEventAbi,
                TestData.Contracts.StandardContract.TransferEventAbi };

            var matcher = new EventMatcher(eventAbis);
            _log.Topics[0] = "0xedf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef";
            Assert.False(matcher.IsMatch(_log));
        }

        [Fact]
        public void GivenOnlyTheAddressMatcher_AddressMatcherSuccessReturnsTrue()
        {
            var addressMatcher = new Mock<IEventAddressMatcher>();
            addressMatcher.Setup(a => a.IsMatch(_log)).Returns(true);
            var matcher = new EventMatcher(addressMatcher: addressMatcher.Object);
            Assert.True(matcher.IsMatch(_log));
        }

        [Fact]
        public void GivenOnlyTheAddressMatcher_WhenAddressMatcherFailReturnsFalse()
        {
            var addressMatcher = new Mock<IEventAddressMatcher>();
            addressMatcher.Setup(a => a.IsMatch(_log)).Returns(false);
            var matcher = new EventMatcher(addressMatcher: addressMatcher.Object);
            Assert.False(matcher.IsMatch(_log));
        }

        [Fact]
        public void GivenEventAbiAndAddressMatcher_WhenAbiAndAddressMatch_ReturnsTrue()
        {
            var addressMatcher = new Mock<IEventAddressMatcher>();
            addressMatcher.Setup(a => a.IsMatch(_log)).Returns(true);
            var matcher = new EventMatcher(_eventAbis, addressMatcher.Object);
            Assert.True(matcher.IsMatch(_log));
        }

        [Fact]
        public void GivenAllMatchers_WhenAllMatchersMatch_ReturnsTrue()
        {
            var addressMatcher = new Mock<IEventAddressMatcher>();
            addressMatcher.Setup(a => a.IsMatch(_log)).Returns(true);
            var parameterMatcher = new Mock<IEventParameterMatcher>();
            parameterMatcher.Setup(p => p.IsMatch(_eventAbis, _log)).Returns(true);

            var matcher = new EventMatcher(_eventAbis, addressMatcher.Object, parameterMatcher.Object);
            Assert.True(matcher.IsMatch(_log));
        }

        [Fact]
        public void GivenAllMatchers_WhenEventSignatureDoesNotMatch_ReturnsFalse()
        {
            var addressMatcher = new Mock<IEventAddressMatcher>();
            addressMatcher.Setup(a => a.IsMatch(_log)).Returns(true);
            var parameterMatcher = new Mock<IEventParameterMatcher>();
            parameterMatcher.Setup(p => p.IsMatch(_eventAbis, _log)).Returns(true);


            _log.Topics[0] = "0xedf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef";
            var matcher = new EventMatcher(_eventAbis, addressMatcher.Object, parameterMatcher.Object);
            Assert.False(matcher.IsMatch(_log));
        }

        [Fact]
        public void GivenAllMatchers_WhenParameterDoesNotMatch_ReturnsFalse()
        {
            var addressMatcher = new Mock<IEventAddressMatcher>();
            addressMatcher.Setup(a => a.IsMatch(_log)).Returns(true);
            var parameterMatcher = new Mock<IEventParameterMatcher>();
            parameterMatcher.Setup(p => p.IsMatch(_eventAbis, _log)).Returns(false);

            var matcher = new EventMatcher(_eventAbis, addressMatcher.Object, parameterMatcher.Object);
            Assert.False(matcher.IsMatch(_log));
        }

        [Fact]
        public void GivenAllMatchers_WhenAddressDoesNotMatch_ReturnsFalse()
        {
            var addressMatcher = new Mock<IEventAddressMatcher>();
            addressMatcher.Setup(a => a.IsMatch(_log)).Returns(false);
            var parameterMatcher = new Mock<IEventParameterMatcher>();
            parameterMatcher.Setup(p => p.IsMatch(_eventAbis, _log)).Returns(true);

            var matcher = new EventMatcher(_eventAbis, addressMatcher.Object, parameterMatcher.Object);
            Assert.False(matcher.IsMatch(_log));
        }


    }
}
