using Nethereum.BlockchainProcessing.Processors;
using Nethereum.BlockchainProcessing.Processors.Transactions;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing
{
    public class ContractSpecificFilterBuilderTests
    {
        protected internal const string ContractAddress1 = "0xC03cDD393C89D169bd4877d58f0554f320f21037";
        protected internal const string ContractAddress2 = "0xE03cDD393C89D169bd4877d58f0554f320f21037";
        protected internal const string AnotherAddress = "0xD03cDD393C89D169bd4877d58f0554f320f21037";

        [Fact]
        public async Task Includes_Contract_Creation_Transaction()
        {
            var builder = new ContractSpecificFilterBuilder(ContractAddress1);
            
            // contract creation tx
            Assert.True(await builder.TransactionFilter.IsMatchAsync(
                new Transaction {To = null}));

            // contract creation receipt
            Assert.True(await builder.TransactionReceiptFilter.IsMatchAsync(
                new TransactionReceipt() {ContractAddress = ContractAddress1}));
        }

        [Fact]
        public async Task Includes_Calls_To_Contract()
        {
            var builder = new ContractSpecificFilterBuilder(ContractAddress1);

            //tx sent to contract
            Assert.True(await builder.TransactionFilter.IsMatchAsync(
                new Transaction {To = ContractAddress1}));

            // non contract creation transaction
            Assert.True(await builder.TransactionReceiptFilter.IsMatchAsync(
                new TransactionReceipt() {ContractAddress = Extensions.EmptyAddressHex}));
        }

        [Fact]
        public async Task Supports_Multiple_ContractAddresses()
        {
            var builder = new ContractSpecificFilterBuilder(new []{ContractAddress1, ContractAddress2});

            //tx sent to contract 1
            Assert.True(await builder.TransactionFilter.IsMatchAsync(
                new Transaction {To = ContractAddress1}));

            //tx sent to contract 2
            Assert.True(await builder.TransactionFilter.IsMatchAsync(
                new Transaction {To = ContractAddress2}));

            //tx sent to another address
            Assert.False(await builder.TransactionFilter.IsMatchAsync(
                new Transaction {To = AnotherAddress}));
        }

        [Fact]
        public async Task Excludes_Contract_Creation_Calls_For_Other_Contracts()
        {
            var builder = new ContractSpecificFilterBuilder(ContractAddress1);

            var tx = new Transaction {To = null};
            var receipt = new TransactionReceipt {ContractAddress = AnotherAddress};

            Assert.True(await 
                builder.Filters.TransactionFilters.IsMatchAsync((tx)));

            Assert.False(await 
                builder.Filters.TransactionReceiptFilters.IsMatchAsync((receipt)));
        }

        [Fact]
        public async Task Excludes_Transactions_For_Other_Contracts()
        {
            var builder = new ContractSpecificFilterBuilder(ContractAddress1);

            var tx = new Transaction {To = AnotherAddress};

            Assert.False(await 
                builder.Filters.TransactionFilters.IsMatchAsync((tx)));

        }

        [Fact]
        public void Creates_TransactionFilter_And_TransactionReceiptFilter()
        {
            var builder = new ContractSpecificFilterBuilder(ContractAddress1);

            Assert.NotNull(builder.TransactionFilter);
            Assert.NotNull(builder.TransactionReceiptFilter);
        }

        [Fact]
        public void Creates_FilterContainer_With_TransactionFilter_And_TransactionReceiptFilter()
        {
            var builder = new ContractSpecificFilterBuilder(ContractAddress1);

            Assert.Single(builder.Filters.TransactionFilters);
            Assert.Single(builder.Filters.TransactionReceiptFilters);

            Assert.Null(builder.Filters.BlockFilters);
            Assert.Null(builder.Filters.TransactionAndReceiptFilters);
            Assert.Null(builder.Filters.TransactionLogFilters);
        }


    }
}
