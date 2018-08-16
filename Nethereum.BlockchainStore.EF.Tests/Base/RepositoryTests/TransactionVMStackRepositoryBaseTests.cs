using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nethereum.BlockchainStore.EF.Repositories;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.EF.Tests.Base.RepositoryTests
{
    public abstract class TransactionVMStackRepositoryBaseTests: RepositoryTestBase
    {
        protected TransactionVMStackRepositoryBaseTests(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        [TestMethod]
        public async Task UpsertAsync()
        {
            var context = contextFactory.CreateContext();
            var repository = new TransactionVMStackRepository(contextFactory);

            const string transactionHash = "0x020af76554bd67c6c716a70bf214eaf7284a483dd8597d7761f78fce11c83a0a";
            const string address = "0xba0ef20713557e1c28e12464e4310dff04c0b3ba";
            var stackTrace = JObject.Parse("{structLogs:['log1', 'log2']}");

            var existingRow = await context.TransactionVmStacks.FindByTransactionHashAync(transactionHash);
            if (existingRow != null)
            {
                context.TransactionVmStacks.Remove(existingRow);
                await context.SaveChangesAsync();
            }

            await repository.UpsertAsync(transactionHash, address, stackTrace);

            var storedRow = await context.TransactionVmStacks.FindByTransactionHashAync(transactionHash);

            Assert.IsNotNull(storedRow);

            Assert.AreEqual(transactionHash, storedRow.TransactionHash);
            Assert.AreEqual(address, storedRow.Address);
            var structLogs = JArray.Parse(storedRow.StructLogs);
            Assert.IsNotNull(structLogs);
            Assert.AreEqual(2, structLogs.Count);
            Assert.AreEqual("log1", structLogs[0]);
            Assert.AreEqual("log2", structLogs[1]);
        }
    }
}
