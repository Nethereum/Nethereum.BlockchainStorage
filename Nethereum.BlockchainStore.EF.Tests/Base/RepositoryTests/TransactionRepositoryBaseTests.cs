using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nethereum.BlockchainStore.EF.Repositories;
using Nethereum.BlockchainStore.Entities;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;
using Utils = Nethereum.BlockchainStore.EF.Tests.Base.Common.Utils;

namespace Nethereum.BlockchainStore.EF.Tests.Base.RepositoryTests
{
    public abstract class TransactionRepositoryBaseTests: RepositoryTestBase
    {
        protected TransactionRepositoryBaseTests(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        static Random _random = new Random();

        [TestMethod]
        public async Task UpsertAsync_1()
        {
            var repo = new TransactionRepository(contextFactory);

            var transaction = CreateDummyTransaction();
            var receipt = CreateDummyReceipt();

            var blockTimestamp = CreateBlockTimestamp();
            var address = "0x9209b29f2094457d3dba62d1953efea58176ba27";
            var error = (string)null;
            var hasVmStack = false;
            var failure = false;

            //initial insert
            await repo.UpsertAsync(transaction, receipt, failure, blockTimestamp, hasVmStack, error);

            var context = contextFactory.CreateContext();
            var storedTransaction = await context.Transactions.FindByBlockNumberAndHashAsync(transaction.BlockNumber, transaction.TransactionHash);

            Assert.IsNotNull(storedTransaction);
            EnsureCorrectStoredValues(transaction, receipt, blockTimestamp, address, error, null, hasVmStack, storedTransaction);
        }

        [TestMethod]
        public async Task UpsertAsync_2()
        {
            var repo = new TransactionRepository(contextFactory);

            var transaction = CreateDummyTransaction();
            var receipt = CreateDummyReceipt();

            var blockTimestamp = CreateBlockTimestamp();
            var address = "0x9209b29f2094457d3dba62d1953efea58176ba27";
            var error = (string)null;
            var newContractAddress = "0xbb0ee65f8bb24c5c1ed0f5e65184a4a77e9ffc26";
            var hasVmStack = false;
            var code = "";
            var failure = false;

            await repo.UpsertAsync(newContractAddress, code, transaction, receipt, failure, blockTimestamp);

            var context = contextFactory.CreateContext();
            var storedTransaction = await context.Transactions.FindByBlockNumberAndHashAsync(transaction.BlockNumber, transaction.TransactionHash);

            Assert.IsNotNull(storedTransaction);
            EnsureCorrectStoredValues(transaction, receipt, blockTimestamp, address, error, newContractAddress, hasVmStack, storedTransaction);
        }
        protected static HexBigInteger CreateBlockTimestamp()
        {
            return Utils.CreateBlockTimestamp();
        }

        protected static void EnsureCorrectStoredValues(Transaction transaction, TransactionReceipt receipt, HexBigInteger blockTimestamp, string address, string error, string newContractAddress, bool hasVmStack, TransactionBase storedTransaction)
        {
            Assert.AreEqual(transaction.BlockHash, storedTransaction.BlockHash);
            Assert.AreEqual(transaction.TransactionHash, storedTransaction.Hash);
            Assert.AreEqual(transaction.From, storedTransaction.AddressFrom);
            Assert.AreEqual((long)transaction.TransactionIndex.Value, storedTransaction.TransactionIndex);
            Assert.AreEqual(transaction.Value.Value.ToString(), storedTransaction.Value);
            Assert.AreEqual(transaction.To, storedTransaction.AddressTo);
            Assert.AreEqual(newContractAddress, storedTransaction.NewContractAddress);
            Assert.AreEqual(transaction.BlockNumber.Value.ToString(), storedTransaction.BlockNumber);
            Assert.AreEqual((long)transaction.Gas.Value, storedTransaction.Gas);
            Assert.AreEqual((long)transaction.GasPrice.Value, storedTransaction.GasPrice);
            Assert.AreEqual(transaction.Input, storedTransaction.Input);
            Assert.AreEqual((long)transaction.Nonce.Value, storedTransaction.Nonce);
            Assert.IsFalse(storedTransaction.Failed);
            Assert.AreEqual((long)receipt.GasUsed.Value, storedTransaction.GasUsed);
            Assert.AreEqual((long)receipt.CumulativeGasUsed.Value, storedTransaction.CumulativeGasUsed);
            Assert.IsFalse(storedTransaction.HasLog);
            Assert.AreEqual((long)blockTimestamp.Value, storedTransaction.TimeStamp);
            Assert.AreEqual(hasVmStack, storedTransaction.HasVmStack);

            if(error == null)
                Assert.IsTrue(string.IsNullOrEmpty(storedTransaction.Error));
            else
                Assert.AreEqual(error, storedTransaction.Error);
        }

        protected static TransactionReceipt CreateDummyReceipt()
        {
            return new TransactionReceipt
            {
                TransactionIndex = new HexBigInteger(0),
                GasUsed = new HexBigInteger(75),
                CumulativeGasUsed = new HexBigInteger(90),
                Logs = new JArray()
            };
        }

        protected static Transaction CreateDummyTransaction()
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
                TransactionHash = "0xcb00b69d2594a3583309f332ada97d0df48bae00170e36a4f7bbdad7783fc7e5",
                TransactionIndex = new HexBigInteger(0)
            };
        }

    }
}
