using System;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests
{
    public class RegularExpressionsTests
    {
        [Theory]
        [InlineData("0", SearchType.Block)]
        [InlineData("7087086", SearchType.Block)]

        [InlineData("0x3c81039c578811a85742f2476da90e363a88ca93763db4a194e35367d9a72fd8", SearchType.Transaction)]
        [InlineData("0x3C81039C578811A85742F2476DA90E363A88CA93763DB4A194E35367D9A72FD8", SearchType.Transaction)]
        [InlineData("0X3C81039C578811A85742F2476DA90E363A88CA93763DB4A194E35367D9A72FD8", SearchType.Transaction)]

        [InlineData("0x007EFD2FB97f65D4F805c3662d4d20BaE6e08d9B", SearchType.Address)]
        [InlineData("0x007efd2fb97f65d4f805c3662d4d20bae6e08d9b", SearchType.Address)]
        [InlineData("0X007efd2fb97f65d4f805c3662d4d20bae6e08d9b", SearchType.Address)]

        [InlineData("", SearchType.Unknown)]
        [InlineData("  ", SearchType.Unknown)]
        [InlineData("test", SearchType.Unknown)]
        [InlineData("-1", SearchType.Unknown)]
        [InlineData("007EFD2FB97f65D4F805c3662d4d20BaE6e08d9B", SearchType.Unknown)]
        public void InferSearchTypeFromQuery_MatchesQueryToSearchResult(string query, SearchType expectedSearchType)
        {
            Assert.Equal(expectedSearchType, query.InferResultType());
        }
    }
}
