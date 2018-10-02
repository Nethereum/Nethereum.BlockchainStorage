using Nethereum.BlockchainStore.Processors;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Tests.Processing
{
    public class ExtensionTests
    {
        [Theory]
        [InlineData("0x0")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void EmptyAddress(string address)
        {
            Assert.True(address.IsAnEmptyAddress());
            Assert.False(address.IsNotAnEmptyAddress());
        }

        [Theory]
        [InlineData("0x01234")]
        [InlineData("0x9209b29f2094457d3dba62d1953efea58176ba27")]
        public void NotAnEmptyAddress(string address)
        {
            Assert.False(address.IsAnEmptyAddress());
            Assert.True(address.IsNotAnEmptyAddress());
        }

        [Theory]
        [InlineData("", "0x1009b29f2094457d3dba62d1953efea58176ba27")]
        public void IsForContractCreation_ReturnsTrue(string toAddress, string contractAddress)
        {
            var transaction = new Transaction {To = toAddress};
            var receipt = new TransactionReceipt {ContractAddress = contractAddress};

            Assert.True(transaction.IsForContractCreation(receipt));
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("0x9209b29f2094457d3dba62d1953efea58176ba27", "")]
        public void IsForContractCreation_ReturnsFalse(string toAddress, string contractAddress)
        {
            var transaction = new Transaction {To = toAddress};
            var receipt = new TransactionReceipt {ContractAddress = contractAddress};

            Assert.False(transaction.IsForContractCreation(receipt));
        }

        [Fact]
        public async Task Filters_IsMatchAsync_WhenEmpty_ReturnsTrue()
        {
            var filters = new List<IFilter<TestFilterItem>>();
            var item = new TestFilterItem();
            Assert.True(await filters.IsMatchAsync(item));
            Assert.False(await filters.IgnoreAsync(item));
        }

        [Fact]
        public async Task Filters_IsMatchAsync_WhenListIsNull_ReturnsTrue()
        {
            List<IFilter<TestFilterItem>> filters = null;
            var item = new TestFilterItem();
            Assert.True(await filters.IsMatchAsync(item));
            Assert.False(await filters.IgnoreAsync(item));
        }

        [Fact]
        public async Task Filters_IsMatchAsync_WhenAFilterMatches_ReturnsTrue()
        {
            var filters = new List<IFilter<TestFilterItem>>();
            var emptyValueFilter = new Filter<TestFilterItem>(
                i => Task.FromResult(string.IsNullOrEmpty(i.Value)));

            filters.Add(emptyValueFilter);
            var item = new TestFilterItem {Value = string.Empty};
            Assert.True(await filters.IsMatchAsync(item));
            Assert.False(await filters.IgnoreAsync(item));
        }

        [Fact]
        public async Task Filters_IsMatchAsync_WhenAnyFilterMatches_ReturnsTrue()
        {
            var filters = new List<IFilter<TestFilterItem>>();
            var emptyValueFilter = new Filter<TestFilterItem>(
                i => Task.FromResult(string.IsNullOrEmpty(i.Value)));

            filters.Add(emptyValueFilter);
            var valueEqualsNethereumFilter = new Filter<TestFilterItem>(
                i => Task.FromResult(i.Value == "Nethereum"));

            filters.Add(valueEqualsNethereumFilter);

            var item = new TestFilterItem {Value = "Nethereum"};
            Assert.True(await filters.IsMatchAsync(item));
            Assert.False(await filters.IgnoreAsync(item));
        }

        [Fact]
        public async Task Filters_IsMatchAsync_WhenNoFilterMatch_ReturnsFalse()
        {
            var filters = new List<IFilter<TestFilterItem>>();
            var noEmptyValuesFilter = new Filter<TestFilterItem>(
                i => Task.FromResult(string.IsNullOrEmpty(i.Value)));

            filters.Add(noEmptyValuesFilter);
            var item = new TestFilterItem {Value = "Not Empty"};
            Assert.False(await filters.IsMatchAsync(item));
            Assert.True(await filters.IgnoreAsync(item));
        }
    }
}
