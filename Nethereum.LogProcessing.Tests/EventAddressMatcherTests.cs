using Nethereum.BlockchainProcessing.Processing.Logs.Matching;
using Nethereum.RPC.Eth.DTOs;
using System;
using Xunit;

namespace Nethereum.LogProcessing.Tests
{
    public class EventAddressMatcherTests
    {
        FilterLog _log = new FilterLog()
        {
            Address = "0x243e72b69141f6af525a9a5fd939668ee9f2b354"
        };

        [Fact]
        public void WhenThereAreNoAddressesSpecifedReturnsTrue()
        {
            var matcher = new EventAddressMatcher(Array.Empty<string>());
            Assert.True(matcher.IsMatch(_log));
        }

        [Fact]
        public void GivenANullAddressListAlwaysReturnsTrue()
        {
            var matcher = new EventAddressMatcher();
            Assert.True(matcher.IsMatch(_log));
        }

        [Fact]
        public void WhenAddressIsNotFoundReturnsFalse()
        {
            var matcher = new EventAddressMatcher(new[] { "0x143e72b69141f6af525a9a5fd939668ee9f2b354" });
            Assert.False(matcher.IsMatch(_log));
        }

        [Fact]
        public void WhenAnyAddressIsFoundReturnsTrue()
        {
            var matcher = new EventAddressMatcher(new[] { "0x143e72b69141f6af525a9a5fd939668ee9f2b354", "0x243e72b69141f6af525a9a5fd939668ee9f2b354" });
            Assert.True(matcher.IsMatch(_log));
        }
    }
}
