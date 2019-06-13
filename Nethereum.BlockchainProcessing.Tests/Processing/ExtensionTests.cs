using Nethereum.BlockchainProcessing.Processors;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using BigInteger = System.Numerics.BigInteger;

namespace Nethereum.BlockchainProcessing.Tests.Processing
{
    public class ExtensionTests
    {
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

        [Fact]
        public void TransactionReceipt_Succeeded_When_Status_Is_One_Returns_True()
        {
            Assert.True(new TransactionReceipt {Status = new HexBigInteger(BigInteger.One)}.Succeeded());
            Assert.False(new TransactionReceipt {Status = new HexBigInteger(BigInteger.Zero)}.Succeeded());
        }

        [Fact]
        public void TransactionReceipt_Failed_When_Status_Is_Not_One_Returns_True()
        {
            Assert.False(new TransactionReceipt {Status = new HexBigInteger(BigInteger.One)}.Failed());
            Assert.True(new TransactionReceipt {Status = new HexBigInteger(BigInteger.Zero)}.Failed());
        }

        [Fact]
        public void Transaction_GetAllRelatedAddresses_Returns_Unique_Addresses_From_Tx_Contract_And_Logs()
        {
            var tx = new Transaction
            {
                From = "0x1009b29f2094457d3dba62d1953efea58176ba27",
                To = "0x2009b29f2094457d3dba62d1953efea58176ba27"
            };

            var receipt = new TransactionReceipt
            {
                ContractAddress = "0x3009b29f2094457d3dba62d1953efea58176ba27"
            };

            var logAddress1 = "0x4009b29f2094457d3dba62d1953efea58176ba27";
            var logAddress2 = "0x5009b29f2094457d3dba62d1953efea58176ba27";

            var log1 = JObject.FromObject(new {address = logAddress1});
            var log2 = JObject.FromObject(new {address = logAddress2});
            var duplicateLogAddress = JObject.FromObject(new {address = logAddress2});

            receipt.Logs = new JArray(log1, log2, duplicateLogAddress);

            var addresses = tx.GetAllRelatedAddresses(receipt);

            Assert.Equal(5, addresses.Length);
            Assert.Contains(tx.From, addresses);
            Assert.Contains(tx.To, addresses);
            Assert.Contains(receipt.ContractAddress, addresses);
            Assert.Contains(logAddress1, addresses);
            Assert.Contains(logAddress2, addresses);
        }

        [Fact]
        public void Transaction_GetAllRelatedAddresses_ToAddress_ContractAddress_And_Logs_Are_Optional()
        {
            var tx = new Transaction
            {
                From = "0x1009b29f2094457d3dba62d1953efea58176ba27"
            };

            var receipt = new TransactionReceipt{ };

            var addresses = tx.GetAllRelatedAddresses(receipt);

            Assert.Single(addresses);
            Assert.Contains(tx.From, addresses);
        }
    }
}
