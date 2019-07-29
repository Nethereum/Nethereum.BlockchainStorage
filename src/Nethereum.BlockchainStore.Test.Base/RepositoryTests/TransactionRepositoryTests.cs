using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.Test.Base.RepositoryTests
{
    public class TransactionRepositoryTests: IRepositoryTest
    {
        static readonly Random _random = new Random();
        private readonly ITransactionRepository _repo;

        public TransactionRepositoryTests(ITransactionRepository repo)
        {
            this._repo = repo;
        }

        public async Task RunAsync()
        {
            await UpsertAsync_1();
            await UpsertAsync_2();
            await EnsureUpsertWorksConcurrently();
        }

        public async Task EnsureUpsertWorksConcurrently()
        {
            var transactions = new List<Tuple<Transaction, TransactionReceipt>>(5);

            for (var i = 0; i < 5; i++)
            {
                var transaction = CreateDummyTransaction(i);
                var receipt = CreateDummyReceipt(i);
                transactions.Add(new Tuple<Transaction, TransactionReceipt>(transaction, receipt));
            }

            var blockTimestamp = CreateBlockTimestamp();
            var address = "0x9209b29f2094457d3dba62d1953efea58176ba27";
            var error = (string)null;
            var hasVmStack = false;
            var failure = false;

            var upsertTasks = from item in transactions
                select _repo.UpsertAsync(new TransactionReceiptVO(
                    block: new RPC.Eth.DTOs.Block { Timestamp = blockTimestamp}, transaction: item.Item1, transactionReceipt: item.Item2, hasError: failure, error: error, hasVmStack: hasVmStack));

            await Task.WhenAll(upsertTasks);

            foreach (var item in transactions)
            {
                var storedTransaction = await _repo.FindByBlockNumberAndHashAsync(item.Item1.BlockNumber, item.Item1.TransactionHash);

                Assert.NotNull(storedTransaction);
                EnsureCorrectStoredValues(item.Item1, item.Item2, blockTimestamp, address, error, null, hasVmStack, storedTransaction);
            }

        }

        public async Task UpsertAsync_1()
        {
            var transaction = CreateDummyTransaction();
            var receipt = CreateDummyReceipt();

            var blockTimestamp = CreateBlockTimestamp();
            var address = "0x9209b29f2094457d3dba62d1953efea58176ba27";
            var error = (string)null;
            var hasVmStack = false;
            var failure = false;

            var transactionReceiptVO = new TransactionReceiptVO(
                    block: new RPC.Eth.DTOs.Block { Timestamp = blockTimestamp }, transaction: transaction, transactionReceipt: receipt, hasError: failure, error: error, hasVmStack: hasVmStack);

            await _repo.UpsertAsync(transactionReceiptVO);
            var storedTransaction = await _repo.FindByBlockNumberAndHashAsync(transaction.BlockNumber, transaction.TransactionHash);

            Assert.NotNull(storedTransaction);
            EnsureCorrectStoredValues(transaction, receipt, blockTimestamp, address, error, null, hasVmStack, storedTransaction);
        }

        public async Task UpsertAsync_2()
        {
            var transaction = CreateDummyTransaction();
            var receipt = CreateDummyReceipt();

            var blockTimestamp = CreateBlockTimestamp();
            var address = "0x9209b29f2094457d3dba62d1953efea58176ba27";
            var error = (string)null;
            var newContractAddress = "0xbb0ee65f8bb24c5c1ed0f5e65184a4a77e9ffc26";
            var hasVmStack = false;
            var code = "";
            var failure = false;
            receipt.ContractAddress = newContractAddress;

            var transactionReceiptVO = new TransactionReceiptVO(
                block: new RPC.Eth.DTOs.Block { Timestamp = blockTimestamp }, transaction: transaction, transactionReceipt: receipt, hasError: failure, error: error, hasVmStack: hasVmStack);


            await _repo.UpsertAsync(transactionReceiptVO, code, failure);

            var storedTransaction = await _repo.FindByBlockNumberAndHashAsync(transaction.BlockNumber, transaction.TransactionHash);

            Assert.NotNull(storedTransaction);
            EnsureCorrectStoredValues(transaction, receipt, blockTimestamp, address, error, newContractAddress, hasVmStack, storedTransaction);
        }



        protected static HexBigInteger CreateBlockTimestamp()
        {
            return Utils.CreateBlockTimestamp();
        }

        protected static void EnsureCorrectStoredValues(
            Transaction transaction, 
            TransactionReceipt receipt, 
            HexBigInteger blockTimestamp, 
            string address, 
            string error, 
            string newContractAddress, 
            bool hasVmStack, 
            ITransactionView storedTransaction)
        {
            Assert.Equal(transaction.BlockHash, storedTransaction.BlockHash);
            Assert.Equal(transaction.TransactionHash, storedTransaction.Hash);
            Assert.Equal(transaction.From, storedTransaction.AddressFrom);
            Assert.Equal(transaction.TransactionIndex.Value.ToString(), storedTransaction.TransactionIndex);
            Assert.Equal(transaction.Value.Value.ToString(), storedTransaction.Value);
            Assert.Equal(transaction.To, storedTransaction.AddressTo);
            Assert.Equal(newContractAddress ?? string.Empty, storedTransaction.NewContractAddress ?? string.Empty);
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
