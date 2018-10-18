using Nethereum.BlockchainProcessing.Processors.Transactions;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing
{
    public class StaticConvenienceFilterTests
    {
        protected internal const string Address1 = "0xC03cDD393C89D169bd4877d58f0554f320f21037";
        protected internal const string Address2 = "0xE03cDD393C89D169bd4877d58f0554f320f21037";
        protected internal const string UnknownAddress = "0xF03cDD393C89D169bd4877d58f0554f320f21037";


        [Fact]
        public async Task TransactionFilter_From_Reports_Matches()
        {
            var tx = new Transaction {From = Address1};
            var filter = TransactionFilter.From(Address1);
            Assert.True(await filter.IsMatchAsync(tx));            
        }

        [Fact]
        public async Task TransactionFilter_FromAndTo_Reports_Matches()
        {
            var tx = new Transaction {From = Address1, To = Address2};
            var filter = TransactionFilter.FromAndTo(Address1, Address2);
            Assert.True(await filter.IsMatchAsync(tx));            
        }

        [Theory]
        [InlineData(Address1, null)]
        [InlineData(null, Address2)]
        [InlineData(Address1, UnknownAddress)]
        [InlineData(UnknownAddress, Address2)]
        public async Task TransactionFilter_FromAndTo_Reports_Non_Matches(string from, string to)
        {
            var tx1 = new Transaction {From = from, To = to};
            var filter = TransactionFilter.FromAndTo(Address1, Address2);
            Assert.False(await filter.IsMatchAsync(tx1));   
        }

        [Fact]
        public async Task TransactionFilter_From_Many_Reports_Matches()
        {
            var tx1 = new Transaction {From = Address1};
            var tx2 = new Transaction {From = Address2};
            var tx3 = new Transaction {From = UnknownAddress};

            var filter = TransactionFilter.From(new []{Address1, Address2});
            Assert.True(await filter.IsMatchAsync(tx1));    
            Assert.True(await filter.IsMatchAsync(tx2));    
            Assert.False(await filter.IsMatchAsync(tx3));    
        }

        [Fact]
        public async Task TransactionFilter_To_Reports_Matches()
        {
            var tx = new Transaction {To = Address1};
            var filter = TransactionFilter.To(Address1);
            Assert.True(await filter.IsMatchAsync(tx));            
        }

        [Fact]
        public async Task TransactionFilter_To_Reports_Non_Matches()
        {
            var tx = new Transaction {To = Address2};
            var filter = TransactionFilter.To(Address1);
            Assert.False(await filter.IsMatchAsync(tx));            
        }

        [Fact]
        public async Task TransactionFilter_ToOrEmpty_When_Address_Matches_Returns_True()
        {
            var tx = new Transaction {To = Address1};
            var filter = TransactionFilter.ToOrEmpty(Address1);
            Assert.True(await filter.IsMatchAsync(tx));            
        }

        [Fact]
        public async Task TransactionFilter_ToOrEmpty_ForManyAddresses_When_Address_Matches_Returns_True()
        {
            var filter = TransactionFilter.ToOrEmpty(new []{Address1, Address2});
            var tx1 = new Transaction {To = Address1};
            var tx2 = new Transaction {To = Address2};
            var tx3 = new Transaction {To = UnknownAddress};

            Assert.True(await filter.IsMatchAsync(tx1));       
            Assert.True(await filter.IsMatchAsync(tx2));       
            Assert.False(await filter.IsMatchAsync(tx3));       
        }

        [Fact]
        public async Task TransactionFilter_ToOrEmpty_When_Address_IsEmpty_Returns_True()
        {
            var tx = new Transaction {To = null};
            var filter = TransactionFilter.ToOrEmpty(Address1);
            Assert.True(await filter.IsMatchAsync(tx));            
        }

        [Fact]
        public async Task TransactionFilter_ToOrEmpty_When_Address_Is_Different_Returns_False()
        {
            var tx = new Transaction {To = Address2};
            var filter = TransactionFilter.ToOrEmpty(Address1);
            Assert.False(await filter.IsMatchAsync(tx));            
        }

        [Fact]
        public async Task TransactionReceiptFilter_ForContract_Matches()
        {
            var tx = new TransactionReceipt{ContractAddress = Address1};
            var filter = TransactionReceiptFilter.ForContract(Address1);
            Assert.True(await filter.IsMatchAsync(tx));            
        }

        [Fact]
        public async Task TransactionReceiptFilter_ForContract_Supports_Multiple_Contract_Addresses()
        {
            var tx1 = new TransactionReceipt{ContractAddress = Address1};
            var tx2 = new TransactionReceipt{ContractAddress = Address2};
            var tx3 = new TransactionReceipt{ContractAddress = UnknownAddress};
            var tx4 = new TransactionReceipt{ContractAddress = null};

            var filter = TransactionReceiptFilter.ForContract(new []{ Address1, Address2});
            Assert.True(await filter.IsMatchAsync(tx1));  
            Assert.True(await filter.IsMatchAsync(tx2));  
            Assert.False(await filter.IsMatchAsync(tx3));  
            Assert.False(await filter.IsMatchAsync(tx4));  
        }

        [Fact]
        public async Task TransactionReceiptFilter_ForContractOrEmpty_Matches_Address()
        {
            var tx = new TransactionReceipt{ContractAddress = Address1};
            var filter = TransactionReceiptFilter.ForContractOrEmpty(Address1);
            Assert.True(await filter.IsMatchAsync(tx));            
        }

        [Fact]
        public async Task TransactionReceiptFilter_ForContractOrEmpty_Does_Not_Matches_Different_Address()
        {
            var tx = new TransactionReceipt{ContractAddress = Address2};
            var filter = TransactionReceiptFilter.ForContractOrEmpty(Address1);
            Assert.False(await filter.IsMatchAsync(tx));            
        }

        [Fact]
        public async Task TransactionReceiptFilter_ForContractOrEmpty_Matches_Empty_Address()
        {
            var tx = new TransactionReceipt{ContractAddress = null};
            var filter = TransactionReceiptFilter.ForContractOrEmpty(Address1);
            Assert.True(await filter.IsMatchAsync(tx));            
        }

        [Fact]
        public async Task TransactionReceiptFilter_ForContractOrEmpty_Supports_Multiple_Addresses()
        {
            var tx1 = new TransactionReceipt{ContractAddress = Address1};
            var tx2 = new TransactionReceipt{ContractAddress = Address2};
            var tx3 = new TransactionReceipt{ContractAddress = UnknownAddress};
            var tx4 = new TransactionReceipt{ContractAddress = null};

            var filter = TransactionReceiptFilter.ForContractOrEmpty(new []{Address1, Address2});
            Assert.True(await filter.IsMatchAsync(tx1)); 
            Assert.True(await filter.IsMatchAsync(tx2)); 
            Assert.False(await filter.IsMatchAsync(tx3)); 
            Assert.True(await filter.IsMatchAsync(tx4));  
        }

        [Fact]
        public async Task TransactionReceiptFilter_ForContract_Filters_Out_Unknown_Addresses()
        {
            var tx = new TransactionReceipt{ContractAddress = UnknownAddress};
            var filter = TransactionReceiptFilter.ForContract(Address1);
            Assert.False(await filter.IsMatchAsync(tx));            
        }

        [Fact]
        public async Task TransactionReceiptFilter_ForContract_Filters_Out_Empty_Addresses()
        {
            var tx = new TransactionReceipt{ContractAddress = null};
            var filter = TransactionReceiptFilter.ForContract(Address1);
            Assert.False(await filter.IsMatchAsync(tx));            
        }

        [Fact]
        public async Task TransactionAndReceiptFilter_SentToOrCreatedContract_Supports_Tx_That_Created_Contract()
        {
            var tx = new Transaction {To = null};
            var receipt = new TransactionReceipt{ContractAddress = Address1};
            var filter = TransactionAndReceiptFilter.SentToOrCreatedContract(Address1);
            Assert.True(await filter.IsMatchAsync((tx, receipt)));  
        }

        [Fact]
        public async Task TransactionAndReceiptFilter_SentToOrCreatedContract_Supports_Tx_Sent_To_Contract()
        {
            var tx = new Transaction {To = Address1};
            var receipt = new TransactionReceipt{ContractAddress = null};
            var filter = TransactionAndReceiptFilter.SentToOrCreatedContract(Address1);
            Assert.True(await filter.IsMatchAsync((tx, receipt)));  
        }

        [Fact]
        public async Task TransactionAndReceiptFilter_SentToOrCreatedContract_Excludes_Other_Addresses()
        {
            var tx = new Transaction {To = UnknownAddress};
            var receipt = new TransactionReceipt{ContractAddress = null};
            var filter = TransactionAndReceiptFilter.SentToOrCreatedContract(Address1);
            Assert.False(await filter.IsMatchAsync((tx, receipt)));  
        }

        [Fact]
        public async Task TransactionAndReceiptFilter_SentToOrCreatedContract_Excludes_Other_Contract_Creations()
        {
            var tx = new Transaction {To = null};
            var receipt = new TransactionReceipt{ContractAddress = UnknownAddress};
            var filter = TransactionAndReceiptFilter.SentToOrCreatedContract(Address1);
            Assert.False(await filter.IsMatchAsync((tx, receipt)));  
        }
    }
}
