using Nethereum.BlockchainStore.AzureTables.Entities;
using Nethereum.BlockchainStore.AzureTables.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Xunit;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.AzureTables.Tests.RepositoryTests
{
    [Collection("AzureTablesFixture")]
    public class AddressTransactionRepositoryTests
    {
        static Random _random = new Random();
        private readonly IAzureTableAddressTransactionRepository _repo;

        public AddressTransactionRepositoryTests(AzureTablesFixture fixture)
        {
            this._repo = fixture.Factory.CreateAzureTablesAddressTransactionRepository();
        }

        [Fact]
        public async Task UpsertAsync()
        {
            var transaction = CreateDummyTransaction();
            var receipt = CreateDummyReceipt();

            var blockTimestamp = CreateBlockTimestamp();
            var address = "0x9209b29f2094457d3dba62d1953efea58176ba27";
            var error = (string)null;
            var hasVmStack = false;
            var failure = false;

            await _repo.UpsertAsync(transaction, receipt, failure, blockTimestamp, address, error, hasVmStack);
            AddressTransaction storedTransaction = await _repo.FindByBlockNumberAndHashAsync(transaction.BlockNumber, transaction.TransactionHash);

            Assert.NotNull(storedTransaction);
            EnsureCorrectStoredValues(transaction, receipt, blockTimestamp, address, error, null, hasVmStack, storedTransaction);
        }

        protected static HexBigInteger CreateBlockTimestamp()
        {
            return Utils.CreateBlockTimestamp();
        }

        protected static void EnsureCorrectStoredValues(Transaction transaction, TransactionReceipt receipt, HexBigInteger blockTimestamp, string address, string error, string newContractAddress, bool hasVmStack, AddressTransaction storedTransaction)
        {
            Assert.Equal(transaction.BlockHash, storedTransaction.BlockHash);
            Assert.Equal(transaction.TransactionHash, storedTransaction.Hash);
            Assert.Equal(transaction.From, storedTransaction.AddressFrom);
            Assert.Equal((long)transaction.TransactionIndex.Value, storedTransaction.TransactionIndex);
            Assert.Equal(transaction.Value.Value.ToString(), storedTransaction.Value);
            Assert.Equal(transaction.To, storedTransaction.AddressTo);
            Assert.Equal(newContractAddress, storedTransaction.NewContractAddress);
            Assert.Equal(transaction.BlockNumber.Value.ToString(), storedTransaction.BlockNumber);
            Assert.Equal((long)transaction.Gas.Value, storedTransaction.Gas);
            Assert.Equal((long)transaction.GasPrice.Value, storedTransaction.GasPrice);
            Assert.Equal(transaction.Input, storedTransaction.Input);
            Assert.Equal((long)transaction.Nonce.Value, storedTransaction.Nonce);
            Assert.False(storedTransaction.Failed);
            Assert.Equal((long)receipt.GasUsed.Value, storedTransaction.GasUsed);
            Assert.Equal((long)receipt.CumulativeGasUsed.Value, storedTransaction.CumulativeGasUsed);
            Assert.False(storedTransaction.HasLog);
            Assert.Equal((long)blockTimestamp.Value, storedTransaction.TimeStamp);
            Assert.Equal(hasVmStack, storedTransaction.HasVmStack);

            if(error == null)
                Assert.True(string.IsNullOrEmpty(storedTransaction.Error));
            else
                Assert.Equal(error, storedTransaction.Error);
        }

        protected static TransactionReceipt CreateDummyReceipt(int? txIndex = null)
        {
            return new TransactionReceipt
            {
                TransactionIndex = new HexBigInteger(txIndex ?? 0),
                GasUsed = new HexBigInteger(75),
                CumulativeGasUsed = new HexBigInteger(90),
                Logs = new JArray()
            };
        }

        protected static Transaction CreateDummyTransaction(int? counter = null)
        {
            return new Transaction
            {
                Value = new HexBigInteger(1000),
                Gas = new HexBigInteger(100),
                BlockNumber = new HexBigInteger(_random.Next(1, 10000)),
                GasPrice = new HexBigInteger(50),
                BlockHash = "0xeda749446d157c1d7482c93b6dc14ffc9612b647273c009c08eac70598fc507d",
                From = "0xe6de16a66e5cd7270cc36a851818bc092884fe64",
                Input = "0x3f97cddc0000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000004000000000000000000000000000000000000000000000000000000000000000085368686868686868000000000000000000000000000000000000000000000000",
                Nonce = new HexBigInteger(4),
                To = "0xe6de16a66e5cd7270cc36a851818bc092884fe64",
                TransactionHash = "0xcb00b69d2594a3583309f332ada97d0df48bae00170e36a4f7bbdad7783fc7e" + counter ?? "5",
                TransactionIndex = new HexBigInteger(counter ?? 0)
            };
        }

    }
}
