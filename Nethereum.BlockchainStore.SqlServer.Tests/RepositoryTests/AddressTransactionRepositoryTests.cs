using System.Threading.Tasks;
using Nethereum.BlockchainStore.SqlServer.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Xunit;

namespace Nethereum.BlockchainStore.SqlServer.Tests.RepositoryTests
{
    public class AddressTransactionRepositoryTests: TransactionBaseRepositoryTests
    {
        [Fact]
        public async Task UpsertAsync()
        {
            //setup
            var contextFactory = Utils.CreateDbContextFactory();
            var repo = new AddressTransactionRepository(contextFactory);

            Transaction transaction = CreateDummyTransaction();
            TransactionReceipt receipt = CreateDummyReceipt();

            var blockTimestamp = CreateBlockTimestamp();
            var address = "0x9209b29f2094457d3dba62d1953efea58176ba27";
            var error = (string)null;
            var newContractAddress = "0xbb0ee65f8bb24c5c1ed0f5e65184a4a77e9ffc26";
            var hasVmStack = false;

            //initial insert
            //execute
            await repo.UpsertAsync(transaction, receipt, false, blockTimestamp, address, error, hasVmStack, newContractAddress);

            //assert
            var context = contextFactory.CreateContext();

            var storedTransaction = await context.AddressTransactions.FindByBlockNumberAndHashAsync(transaction.BlockNumber, transaction.TransactionHash);
            Assert.NotNull(storedTransaction);

            EnsureStoredValues(transaction, receipt, blockTimestamp, address, error, newContractAddress, hasVmStack, storedTransaction);

            //update
            await repo.UpsertAsync(transaction, receipt, false, blockTimestamp, address, error, hasVmStack, newContractAddress);
            storedTransaction = await context.AddressTransactions.FindByBlockNumberAndHashAsync(transaction.BlockNumber, transaction.TransactionHash);
            Assert.NotNull(storedTransaction); EnsureStoredValues(transaction, receipt, blockTimestamp, address, error, newContractAddress, hasVmStack, storedTransaction);

        }

        private static void EnsureStoredValues(Transaction transaction, TransactionReceipt receipt, HexBigInteger blockTimestamp, string address, string error, string newContractAddress, bool hasVmStack, Entities.AddressTransaction storedTransaction)
        {
            Assert.Equal(address, storedTransaction.Address);
            EnsureCorrectStoredValues(transaction, receipt, blockTimestamp, address, error, newContractAddress, hasVmStack, storedTransaction);
        }
    }
}
