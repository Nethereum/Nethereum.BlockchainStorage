using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using System;
using System.Numerics;
using Xunit;
using Extensions = Nethereum.RPC.Eth.DTOs.Extensions;

namespace Nethereum.BlockchainProcessing.Tests.DtoExtensions
{
    public class DtoExtensionTests
    {
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
        [InlineData("0x0")]
        public void IsAnEmptyAddress(string address)
        {
            Assert.True(address.IsAnEmptyAddress());
        }

        [Theory]
        [InlineData("0x1009b29f2094457d3dba62d1953efea58176ba27")]
        public void IsNotAnEmptyAddress(string address)
        {
            Assert.True(address.IsNotAnEmptyAddress());
        }

        [Theory]
        [InlineData("0x0", "0x0")]
        [InlineData(null, "0x0")]
        [InlineData("", null)]
        [InlineData("0x0", "")]
        [InlineData(" ", " ")]
        [InlineData("0x1009b29f2094457d3dba62d1953efea58176ba27", "0x1009b29f2094457d3dba62d1953efea58176ba27")]
        [InlineData("0x1009B29F2094457D3DBA62d1953EFEA58176Ba27", "0x1009b29f2094457d3dba62d1953efea58176ba27")]
        public void EqualsAddress(string address1, string address2)
        {
            Assert.True(address1.EqualsAddress(address2));
            Assert.True(address2.EqualsAddress(address1));
        }

        [Theory]
        [InlineData(null, "1234")]
        [InlineData("0x2009b29f2094457d3dba62d1953efea58176ba27", "0x1009b29f2094457d3dba62d1953efea58176ba27")]
        public void EqualsAddress_Should_Return_False(string address1, string address2)
        {
            Assert.False(address1.EqualsAddress(address2));
            Assert.False(address2.EqualsAddress(address1));
        }

        [Theory]
        [InlineData("0x2009b29f2094457d3dba62d1953efea58176ba27")]
        public void AddressOrEmpty(string address)
        {
            Assert.Equal(address, address.AddressValueOrEmpty());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("0x0")]
        public void AddressValueOrEmpty_Returns_EmptyAddressHex(string address)
        {
            Assert.Equal(Extensions.EmptyAddressHex, address.AddressValueOrEmpty());
        }

        [Theory]
        [InlineData(null)]
        public void Transaction_IsToAnEmptyAddress(string address)
        {
            var tx = new Transaction {To = address};
            Assert.True(tx.IsToAnEmptyAddress());
        }

        [Theory]
        [InlineData("0x2009b29f2094457d3dba62d1953efea58176ba27")]
        public void Transaction_IsToAnEmptyAddress_Returns_False(string address)
        {
            var tx = new Transaction {To = address};
            Assert.False(tx.IsToAnEmptyAddress());
        }

        [Theory]
        [InlineData("0x2009b29f2094457d3dba62d1953efea58176ba27", "0x2009b29f2094457d3dba62d1953efea58176ba27")]
        public void Transaction_IsTo(string txAddress, string address)
        {
            var tx = new Transaction {To = txAddress};
            Assert.True(tx.IsTo(address));
        }

        [Theory]
        [InlineData("0x2009b29f2094457d3dba62d1953efea58176ba27", "0x3009b29f2094457d3dba62d1953efea58176ba27")]
        [InlineData("", "0x3009b29f2094457d3dba62d1953efea58176ba27")]
        public void Transaction_IsTo_Returns_False(string txAddress, string address)
        {
            var tx = new Transaction {To = txAddress};
            Assert.False(tx.IsTo(address));
        }

        [Theory]
        [InlineData("0x2009b29f2094457d3dba62d1953efea58176ba27", "0x2009b29f2094457d3dba62d1953efea58176ba27")]
        public void Transaction_IsFrom(string txAddress, string address)
        {
            var tx = new Transaction {From = txAddress};
            Assert.True(tx.IsFrom(address));
        }

        [Theory]
        [InlineData("0x2009b29f2094457d3dba62d1953efea58176ba27", "0x3009b29f2094457d3dba62d1953efea58176ba27")]
        [InlineData("", "0x3009b29f2094457d3dba62d1953efea58176ba27")]
        public void Transaction_IsFrom_Returns_False(string txAddress, string address)
        {
            var tx = new Transaction {To = txAddress};
            Assert.False(tx.IsFrom(address));
        }

        [Theory]
        [InlineData("0x2009b29f2094457d3dba62d1953efea58176ba27", "0x3009b29f2094457d3dba62d1953efea58176ba27")]
        public void Transaction_IsFromAndTo(string from, string to)
        {
            var tx = new Transaction {From = from, To = to};
            Assert.True(tx.IsFromAndTo(from, to));
        }

        [Theory]
        [InlineData(
            "0x2009b29f2094457d3dba62d1953efea58176ba27", 
            "0x3009b29f2094457d3dba62d1953efea58176ba27",
            "0x9009b29f2094457d3dba62d1953efea58176ba27",
            "0x7009b29f2094457d3dba62d1953efea58176ba27")]
        public void Transaction_IsFromAndTo_Returns_False(string txFrom, string txTo, string from, string to)
        {
            var tx = new Transaction {From = txFrom, To = txTo};
            Assert.False(tx.IsFromAndTo(from, to));
        }

        [Theory]
        [InlineData("0x7009b29f2094457d3dba62d1953efea58176ba27", "0x7009b29f2094457d3dba62d1953efea58176ba27")]
        [InlineData("0x7009b29f2094457d3dba62d1953efea58176ba27", null)]
        [InlineData("0x7009b29f2094457d3dba62d1953efea58176ba27", "0x0")]
        [InlineData("0x7009b29f2094457d3dba62d1953efea58176ba27", "")]
        [InlineData("0x7009b29f2094457d3dba62d1953efea58176ba27", " ")]
        public void TransactionReceipt_IsContractAddressEmptyOrEqual(string address, string contractAddress)
        {
            Assert.True(new TransactionReceipt{ContractAddress = contractAddress}
                .IsContractAddressEmptyOrEqual(address));
        }

        [Theory]
        [InlineData("0x7009b29f2094457d3dba62d1953efea58176ba27", "0x7009b29f2094457d3dba62d1953efea58176ba27")]
        [InlineData("0x7009b29f2094457d3dba62d1953efea58176ba27", "0x7009B29f2094457d3dba62d1953efea58176ba27")]
        [InlineData("0x0", "0x0")]
        [InlineData("", "")]
        [InlineData("", "0x0")]
        [InlineData(null, "0x0")]
        public void TransactionReceipt_IsContractAddressEqual(string address, string contractAddress)
        {
            Assert.True(new TransactionReceipt{ContractAddress = contractAddress}
                .IsContractAddressEqual(address));
        }

        [Theory]
        [InlineData("0x7009b29f2094457d3dba62d1953efea58176ba27", "0x1009b29f2094457d3dba62d1953efea58176ba27")]
        public void TransactionReceipt_IsContractAddressEmptyOrEqual_Returns_False(string address, string contractAddress)
        {
            Assert.False(new TransactionReceipt{ContractAddress = contractAddress}
                .IsContractAddressEmptyOrEqual(address));
        }

        [Theory]
        [InlineData("", "0x1009b29f2094457d3dba62d1953efea58176ba27")]
        public void Transaction_IsForContractCreation_ReturnsTrue(string toAddress, string contractAddress)
        {
            var transaction = new Transaction {To = toAddress};
            var receipt = new TransactionReceipt {ContractAddress = contractAddress};

            Assert.True(transaction.IsForContractCreation(receipt));
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("0x9209b29f2094457d3dba62d1953efea58176ba27", "")]
        public void Transaction_IsForContractCreation_ReturnsFalse(string toAddress, string contractAddress)
        {
            var transaction = new Transaction {To = toAddress};
            var receipt = new TransactionReceipt {ContractAddress = contractAddress};

            Assert.False(transaction.IsForContractCreation(receipt));
        }

        [Fact]
        public void TransactionReceipt_Succeeded()
        {
            Assert.True(new TransactionReceipt{Status = new HexBigInteger(BigInteger.One)}.Succeeded());
        }

        [Fact]
        public void TransactionReceipt_Failed()
        {
            Assert.True(new TransactionReceipt{Status = new HexBigInteger(0)}.Failed());
        }

        [Fact]
        public void TransactionReceipt_HasLogs()
        {
            var logs = JArray.FromObject(new[] {"fake log1", "fake log2"});
            Assert.True(new TransactionReceipt{Logs = logs}.HasLogs());
        }

        [Fact]
        public void TransactionReceipt_HasLogs_When_Log_Array_Is_Null_Or_Empty_Returns_False()
        {
            Assert.False(new TransactionReceipt{Logs = null}.HasLogs());
            var logs = JArray.FromObject(Array.Empty<string>());
            Assert.False(new TransactionReceipt{Logs = logs}.HasLogs());
        }

    }
}
