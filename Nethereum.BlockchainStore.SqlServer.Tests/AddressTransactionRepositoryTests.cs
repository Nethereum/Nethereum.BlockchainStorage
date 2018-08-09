using Nethereum.BlockchainStore.SqlServer.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using NBitcoin.BouncyCastle.Math;
using Xunit;

namespace Nethereum.BlockchainStore.SqlServer.Tests
{
    public class AddressTransactionRepositoryTests
    {
        [Fact]
        public async Task UpsertAsync()
        {
            const string connectionString =
                "Data Source=davewhiffin.database.windows.net;Database=BlockchainStorage;Integrated Security=False;User ID=localhost1;Password=MeLLfMA1wBlJCzSGZhkO;Connect Timeout=60;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            var contextFactory = new BlockchainDbContextFactory(connectionString, null);
            var repo = new AddressTransactionRepository(contextFactory);

            var transaction = new RPC.Eth.DTOs.Transaction
            {
                Value = new HexBigInteger(1000),
                Gas = new HexBigInteger(100),
                BlockNumber = new HexBigInteger(3),
                GasPrice = new HexBigInteger(50),
                BlockHash = "0xeda749446d157c1d7482c93b6dc14ffc9612b647273c009c08eac70598fc507d",
                From = "0xe6de16a66e5cd7270cc36a851818bc092884fe64",
                Input = "0x3f97cddc0000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000004000000000000000000000000000000000000000000000000000000000000000085368686868686868000000000000000000000000000000000000000000000000",
                Nonce = new HexBigInteger(4),
                To = "0xe6de16a66e5cd7270cc36a851818bc092884fe64",
                TransactionHash = "0xcb00b69d2594a3583309f332ada97d0df48bae00170e36a4f7bbdad7783fc7e5",
                TransactionIndex = new HexBigInteger(0)
            };

            var receipt = new TransactionReceipt
            {
                TransactionIndex = new HexBigInteger(0),
                GasUsed = new HexBigInteger(75),
                CumulativeGasUsed = new HexBigInteger(90),
                Logs = new JArray()
            };

            var blockTimestamp = new HexBigInteger(DateTimeOffset.UnixEpoch.ToUnixTimeSeconds());
            var address = "0x9209b29f2094457d3dba62d1953efea58176ba27";
            var error = (string)null;
            var newContractAddress = "0xbb0ee65f8bb24c5c1ed0f5e65184a4a77e9ffc26";
            var hasVmStack = false;


            //initial insert
            await repo.UpsertAsync(transaction, receipt, false, blockTimestamp, address, error, hasVmStack, newContractAddress);

            var context = contextFactory.CreateContext();

            var storedTransaction = await context.AddressTransactions.FindByBlockNumberAndHashAsync(transaction.BlockNumber, transaction.TransactionHash);
            Assert.NotNull(storedTransaction);

            EnsureCorrectStoredValues(transaction, receipt, blockTimestamp, address, error, newContractAddress, hasVmStack, storedTransaction);

            //update
            await repo.UpsertAsync(transaction, receipt, false, blockTimestamp, address, error, hasVmStack, newContractAddress);
            storedTransaction = await context.AddressTransactions.FindByBlockNumberAndHashAsync(transaction.BlockNumber, transaction.TransactionHash);
            Assert.NotNull(storedTransaction);EnsureCorrectStoredValues(transaction, receipt, blockTimestamp, address, error, newContractAddress, hasVmStack, storedTransaction);

        }

        private static void EnsureCorrectStoredValues(Transaction transaction, TransactionReceipt receipt, HexBigInteger blockTimestamp, string address, string error, string newContractAddress, bool hasVmStack, Entities.AddressTransaction storedTransaction)
        {
            Assert.Equal(address, storedTransaction.Address);
            Assert.Equal(transaction.BlockHash, storedTransaction.BlockHash);
            Assert.Equal(transaction.TransactionHash, storedTransaction.Hash);
            Assert.Equal(transaction.From, storedTransaction.AddressFrom);
            Assert.Equal((long)transaction.TransactionIndex.Value, storedTransaction.TransactionIndex);
            Assert.Equal(transaction.Value.ToString(), storedTransaction.Value);
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
    }
}
