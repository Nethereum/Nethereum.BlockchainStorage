using Nethereum.BlockchainProcessing.Storage.Entities;
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
        static readonly Random _random = new Random();
        private readonly AddressTransactionRepository _repo;

        public AddressTransactionRepositoryTests(AzureTablesFixture fixture)
        {
            this._repo = fixture.Factory.CreateAddressTransactionRepository() as AddressTransactionRepository;
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

            var vo = new TransactionReceiptVO(new RPC.Eth.DTOs.Block { Timestamp = blockTimestamp }, transaction, receipt, failure, error, hasVmStack);

            await _repo.UpsertAsync(vo, address, error);
            var storedTransaction = await _repo.FindByAddressBlockNumberAndHashAsync(address, transaction.BlockNumber, transaction.TransactionHash);

            Assert.NotNull(storedTransaction);
            EnsureCorrectStoredValues(transaction, receipt, blockTimestamp, address, error, null, hasVmStack, storedTransaction);

            var txView = await _repo.FindAsync(address, transaction.BlockNumber, transaction.TransactionHash);

            Assert.Equal(address, txView.Address);
            Assert.Equal(storedTransaction.BlockNumber, txView.BlockNumber);
            Assert.Equal(storedTransaction.Hash, txView.Hash);
        }

        protected static HexBigInteger CreateBlockTimestamp()
        {
            return Test.Base.RepositoryTests.Utils.CreateBlockTimestamp();
        }

        protected static void EnsureCorrectStoredValues(Transaction transaction, TransactionReceipt receipt, HexBigInteger blockTimestamp, string address, string error, string newContractAddress, bool hasVmStack, ITransactionView storedTransaction)
        {
            Assert.Equal(transaction.BlockHash, storedTransaction.BlockHash);
            Assert.Equal(transaction.TransactionHash, storedTransaction.Hash);
            Assert.Equal(transaction.From, storedTransaction.AddressFrom);
            Assert.Equal(transaction.TransactionIndex.Value.ToString(), storedTransaction.TransactionIndex);
            Assert.Equal(transaction.Value.Value.ToString(), storedTransaction.Value);
            Assert.Equal(transaction.To, storedTransaction.AddressTo);
            Assert.Equal(newContractAddress ?? string.Empty, storedTransaction.NewContractAddress);
            Assert.Equal(transaction.BlockNumber.Value.ToString(), storedTransaction.BlockNumber);
            Assert.Equal(transaction.Gas.Value.ToString(), storedTransaction.Gas);
            Assert.Equal(transaction.GasPrice.Value.ToString(), storedTransaction.GasPrice);
            Assert.Equal(transaction.Input, storedTransaction.Input);
            Assert.Equal(transaction.Nonce.Value.ToString(), storedTransaction.Nonce);
            Assert.False(storedTransaction.Failed);
            Assert.Equal(receipt.GasUsed.Value.ToString(), storedTransaction.GasUsed);
            Assert.Equal(receipt.CumulativeGasUsed.Value.ToString(), storedTransaction.CumulativeGasUsed);
            Assert.False(storedTransaction.HasLog);
            Assert.Equal(blockTimestamp.Value.ToString(), storedTransaction.TimeStamp);
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
