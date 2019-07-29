using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Xunit;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.Test.Base.RepositoryTests
{
    public class AddressTransactionRepositoryTests: IRepositoryTest
    {
        static readonly Random _random = new Random();
        private readonly IAddressTransactionRepository _repo;

        public AddressTransactionRepositoryTests(IAddressTransactionRepository repo)
        {
            this._repo = repo;
        }

        public async Task RunAsync()
        {
            await UpsertAsync_1();
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

            var vo = new TransactionReceiptVO(new RPC.Eth.DTOs.Block { Timestamp = blockTimestamp }, transaction, receipt, failure, error, hasVmStack);

            await _repo.UpsertAsync(vo, address, error, null);
            var storedTransaction = await _repo.FindAsync(address, transaction.BlockNumber, transaction.TransactionHash);

            Assert.NotNull(storedTransaction);
            EnsureCorrectStoredValues(blockTimestamp, address, transaction, storedTransaction);
        }


        protected static HexBigInteger CreateBlockTimestamp()
        {
            return Utils.CreateBlockTimestamp();
        }

        protected static void EnsureCorrectStoredValues(
            HexBigInteger blockTimestamp, 
            string address, 
            Transaction transaction, 
            IAddressTransactionView storedTransaction)
        {
            Assert.Equal(transaction.TransactionHash, storedTransaction.Hash);
            Assert.Equal(transaction.BlockNumber.Value.ToString(), storedTransaction.BlockNumber);
            Assert.Equal(address, storedTransaction.Address);

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
