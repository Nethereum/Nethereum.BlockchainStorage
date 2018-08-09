using Nethereum.BlockchainStore.SqlServer.Repositories;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.SqlServer.Tests
{
    public class TransactionRepositoryTests: TransactionBaseRepositoryTests
    {
        [Fact]
        public async Task UpsertAsync_1()
        {
            var contextFactory = Utils.CreateDbContextFactory();
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

            Assert.NotNull(storedTransaction);
            EnsureCorrectStoredValues(transaction, receipt, blockTimestamp, address, error, null, hasVmStack, storedTransaction);
        }

        [Fact]
        public async Task UpsertAsync_2()
        {
            var contextFactory = Utils.CreateDbContextFactory();
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

            Assert.NotNull(storedTransaction);
            EnsureCorrectStoredValues(transaction, receipt, blockTimestamp, address, error, newContractAddress, hasVmStack, storedTransaction);
        }


    }
}
