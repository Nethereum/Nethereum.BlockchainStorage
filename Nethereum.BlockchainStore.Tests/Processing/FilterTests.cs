using Nethereum.BlockchainStore.Processors;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Tests.Processing
{
    public class FilterTests
    {
        [Fact]
        public async Task MatchesAsync()
        {
            var filter = new Filter<TestFilterItem>(i => Task.FromResult(i.Value == "target"));
            Assert.True(await filter.IsMatchAsync(new TestFilterItem{Value = "target"}));
            Assert.False(await filter.IsMatchAsync(new TestFilterItem{Value = ""}));
        }
    }
}
