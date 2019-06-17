using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using System;
using System.Numerics;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.DtoExtensions
{
    public class DtoExtensionTests
    {
        public const string Address1 = "0x7009b29f2094457d3dba62d1953efea58176ba27";
        public const string Address2 = "0x1009b29f2094457d3dba62d1953efea58176ba27";
        public const string LowerCaseAddress1 = "0x7009b29f2094457d3dba62d1953efea58176ba27";
        public const string UpperCaseAddress1 = "0x7009B29F2094457D3DBA62D1953EFEA58176BA27";

        [Fact]
        public void BlockWithTransactions_TransactionCount_Returns_Number_Of_Transactions()
        {
            var blockWithTransactions = new BlockWithTransactions { Transactions = new []
            {
                new Transaction()
            }};

            Assert.Equal(blockWithTransactions.Transactions.Length, blockWithTransactions.TransactionCount());
        }

        [Fact]
        public void BlockWithTransactionHashes_TransactionCount_Returns_Length_Of_Hashes()
        {
            var blockWithTransactionHashes = new BlockWithTransactionHashes
            {
                TransactionHashes = new []{"0xc185cc7b9f7862255b82fd41be561fdc94d030567d0b41292008095bf31c39b9"}
            };

            Assert.Equal(blockWithTransactionHashes.TransactionHashes.Length, blockWithTransactionHashes.TransactionCount());
        }

        [Fact]
        public void Block_TransactionCount_Returns_0()
        {
            var block = new Block();
            Assert.Equal(0, block.TransactionCount());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(Address.EmptyAsHex)]
        public void IsAnEmptyAddress_When_Address_Is_Empty_Returns_True(string address)
        {
            Assert.True(address.IsAnEmptyAddress());
        }

        [Theory]
        [InlineData(Address1)]
        public void IsNotAnEmptyAddress_When_Address_Is_Not_Empty_Returns_True(string address)
        {
            Assert.True(address.IsNotAnEmptyAddress());
        }

        [Theory]
        [InlineData(Address.EmptyAsHex, Address.EmptyAsHex)]
        [InlineData(null, Address.EmptyAsHex)]
        [InlineData("", null)]
        [InlineData(Address.EmptyAsHex, "")]
        [InlineData(" ", " ")]
        [InlineData(LowerCaseAddress1, LowerCaseAddress1)]
        [InlineData(UpperCaseAddress1, LowerCaseAddress1)]
        [InlineData(LowerCaseAddress1, UpperCaseAddress1)]
        [InlineData(UpperCaseAddress1, UpperCaseAddress1)]
        public void EqualsAddress_When_Addresses_Are_Equal_Returns_True(string address1, string address2)
        {
            Assert.True(address1.EqualsAddress(address2));
            Assert.True(address2.EqualsAddress(address1));
        }

        [Theory]
        [InlineData(null, Address1)]
        [InlineData(Address.EmptyAsHex, Address1)]
        [InlineData(Address1, Address2)]
        public void EqualsAddress_When_Addresses_Are_Not_Equal_Returns_False(string address1, string address2)
        {
            Assert.False(address1.EqualsAddress(address2));
            Assert.False(address2.EqualsAddress(address1));
        }

        [Theory]
        [InlineData(Address1)]
        [InlineData(Address.EmptyAsHex)]
        public void AddressOrEmpty_When_The_Address_Is_Not_Empty_Returns_The_Address(string address)
        {
            Assert.Equal(address, address.AddressValueOrEmpty());
        }

        [Theory]
        [InlineData(Address.EmptyAsHex)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void AddressOrEmpty_When_The_Address_Is_Empty_Returns_An_Empty_Address(string address)
        {
            Assert.Equal(Address.EmptyAsHex, address.AddressValueOrEmpty());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(Address.EmptyAsHex)]
        public void Transaction_IsToAnEmptyAddress_When_To_Address_Is_Empty_Returns_True(string address)
        {
            var tx = new Transaction {To = address};
            Assert.True(tx.IsToAnEmptyAddress());
        }

        [Theory]
        [InlineData(Address1)]
        public void Transaction_IsToAnEmptyAddress_When_Address_Is_Not_Empty_Returns_False(string address)
        {
            var tx = new Transaction {To = address};
            Assert.False(tx.IsToAnEmptyAddress());
        }

        [Theory]
        [InlineData(Address1, Address1)]
        public void Transaction_IsTo_When_Addresses_Match_Returns_True(string txAddress, string address)
        {
            var tx = new Transaction {To = txAddress};
            Assert.True(tx.IsTo(address));
        }

        [Theory]
        [InlineData(Address1, Address2)]
        [InlineData(Address.EmptyAsHex, Address1)]
        public void Transaction_IsTo_When_Addresses_Differ_Returns_False(string txAddress, string address)
        {
            var tx = new Transaction {To = txAddress};
            Assert.False(tx.IsTo(address));
        }

        [Theory]
        [InlineData(Address1, Address1)]
        public void Transaction_IsFrom_When_Addresses_Match_Returns_True(string txAddress, string address)
        {
            var tx = new Transaction {From = txAddress};
            Assert.True(tx.IsFrom(address));
        }

        [Theory]
        [InlineData(Address1, Address2)]
        [InlineData("", Address1)]
        [InlineData(Address.EmptyAsHex, Address1)]
        public void Transaction_IsFrom_When_Addresses_Differ_Returns_False(string txAddress, string address)
        {
            var tx = new Transaction {To = txAddress};
            Assert.False(tx.IsFrom(address));
        }

        [Theory]
        [InlineData(Address1, Address2)]
        public void Transaction_IsFromAndTo_When_From_And_To_Addresses_Match_Returns_True
            (string from, string to)
        {
            var tx = new Transaction {From = from, To = to};
            Assert.True(tx.IsFromAndTo(from, to));
        }

        [Theory]
        [InlineData(
            Address1, 
            Address2,
            Address2,
            Address1)]
        public void Transaction_IsFromAndTo_When_From_And_To_Addresses_Differ_Returns_False
            (string txFrom, string txTo, string from, string to)
        {
            var tx = new Transaction {From = txFrom, To = txTo};
            Assert.False(tx.IsFromAndTo(from, to));
        }

        [Theory]
        [InlineData(LowerCaseAddress1, LowerCaseAddress1)]
        [InlineData(UpperCaseAddress1, LowerCaseAddress1)]
        [InlineData(LowerCaseAddress1, UpperCaseAddress1)]
        [InlineData(UpperCaseAddress1, UpperCaseAddress1)]
        public void TransactionReceipt_IsContractAddressEmptyOrEqual_When_Addresses_Are_Equal_Returns_True
            (string contractAddress, string address)
        {
            Assert.True(new TransactionReceipt{ContractAddress = contractAddress}
                .IsContractAddressEmptyOrEqual(address));
        }

        [Theory]
        [InlineData(null, Address1)]
        [InlineData(Address.EmptyAsHex, Address1)]
        [InlineData("", Address1)]
        [InlineData(" ", Address1)]
        public void TransactionReceipt_IsContractAddressEmptyOrEqual_When_Contract_Address_Is_Empty_Equal_Returns_True
            (string contractAddress, string address)
        {
            Assert.True(new TransactionReceipt{ContractAddress = contractAddress}
                .IsContractAddressEmptyOrEqual(address));
        }

        [Theory]
        [InlineData(LowerCaseAddress1, LowerCaseAddress1)]
        [InlineData(UpperCaseAddress1, LowerCaseAddress1)]
        [InlineData(LowerCaseAddress1, UpperCaseAddress1)]
        [InlineData(UpperCaseAddress1, UpperCaseAddress1)]
        [InlineData(Address.EmptyAsHex, Address.EmptyAsHex)]
        [InlineData("", "")]
        [InlineData(Address.EmptyAsHex, "")]
        [InlineData(Address.EmptyAsHex, null)]
        public void TransactionReceipt_IsContractAddressEqual_When_Addresses_Are_Equal_Returns_True
            (string contractAddress, string address)
        {
            Assert.True(new TransactionReceipt{ContractAddress = contractAddress}
                .IsContractAddressEqual(address));
        }

        [Theory]
        [InlineData(Address1, Address2)]
        public void TransactionReceipt_IsContractAddressEmptyOrEqual_When_Addresses_Differ_Returns_False
            (string contractAddress, string address)
        {
            Assert.False(new TransactionReceipt{ContractAddress = contractAddress}
                .IsContractAddressEmptyOrEqual(address));
        }

        [Theory]
        [InlineData("", Address1)]
        [InlineData(Address.EmptyAsHex, Address1)]
        [InlineData(null, Address1)]
        public void Transaction_IsForContractCreation_When_To_Address_Is_Empty_And_Contract_Address_Has_Value_Returns_True
            (string toAddress, string contractAddress)
        {
            var transaction = new Transaction {To = toAddress};
            var receipt = new TransactionReceipt {ContractAddress = contractAddress};

            Assert.True(transaction.IsForContractCreation(receipt));
        }

        [Theory]
        [InlineData(Address1, Address.EmptyAsHex)]
        public void Transaction_IsForContractCreation_When_To_Address_Is_Not_Empty_Returns_False
            (string toAddress, string contractAddress)
        {
            var transaction = new Transaction {To = toAddress};
            var receipt = new TransactionReceipt {ContractAddress = contractAddress};

            Assert.False(transaction.IsForContractCreation(receipt));
        }

        [Theory]
        [InlineData("", Address.EmptyAsHex)]
        public void Transaction_IsForContractCreation_When_Contract_Address_Is_Empty_Returns_False
            (string toAddress, string contractAddress)
        {
            var transaction = new Transaction {To = toAddress};
            var receipt = new TransactionReceipt {ContractAddress = contractAddress};

            Assert.False(transaction.IsForContractCreation(receipt));
        }

        [Fact]
        public void TransactionReceipt_Succeeded_When_Status_Equals_1()
        {
            Assert.True(new TransactionReceipt{Status = new HexBigInteger(BigInteger.One)}.Succeeded());
        }

        [Fact]
        public void TransactionReceipt_Failed_When_Status_Is_Zero()
        {
            Assert.True(new TransactionReceipt{Status = new HexBigInteger(BigInteger.Zero)}.Failed());
        }

        [Fact]
        public void TransactionReceipt_When_Status_Is_Null_Default_Is_To_Assume_Success()
        {
            Assert.True(new TransactionReceipt { Status = null }.Succeeded());
            Assert.False(new TransactionReceipt { Status = null }.Failed());
        }

        [Fact]
        public void TransactionReceipt_When_Status_Is_Null_Returns_Treat_As_Null_Flag()
        {
            Assert.True(new TransactionReceipt { Status = null }.Succeeded(treatNullStatusAsFailure: false));
            Assert.False(new TransactionReceipt { Status = null }.Succeeded(treatNullStatusAsFailure: true));
            Assert.False(new TransactionReceipt { Status = null }.Failed(treatNullStatusAsFailure: false));
            Assert.True(new TransactionReceipt { Status = null }.Failed(treatNullStatusAsFailure: true));
        }

        [Fact]
        public void TransactionReceipt_HasLogs_When_Receipt_Logs_Is_Not_Empty_Returns_True()
        {
            var logs = JArray.FromObject(new[] {"fake log1", "fake log2"});
            Assert.True(new TransactionReceipt{Logs = logs}.HasLogs());
        }

        [Fact]
        public void TransactionReceipt_HasLogs_When_Receipt_Logs_Is_Null_Or_Empty_Returns_False()
        {
            Assert.False(new TransactionReceipt{Logs = null}.HasLogs());
            var logs = JArray.FromObject(Array.Empty<string>());
            Assert.False(new TransactionReceipt{Logs = logs}.HasLogs());
        }

    }
}
