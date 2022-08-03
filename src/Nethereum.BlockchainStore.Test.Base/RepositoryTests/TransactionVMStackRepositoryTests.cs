using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Test.Base.RepositoryTests
{
    public class TransactionVMStackRepositoryTests: IRepositoryTest
    {
        private readonly ITransactionVMStackRepository _repo;

        public TransactionVMStackRepositoryTests(ITransactionVMStackRepository repo)
        {
            this._repo = repo;
        }

        public async Task RunAsync()
        {
            await UpsertAsync();
        }

        public async Task UpsertAsync()
        {
            const string transactionHash = "0x020af76554bd67c6c716a70bf214eaf7284a483dd8597d7761f78fce11c83a0a";
            const string address = "0xba0ef20713557e1c28e12464e4310dff04c0b3ba";
            var stackTrace = JObject.Parse("{structLogs:['log1', 'log2']}");

            await _repo.UpsertAsync(transactionHash, address, stackTrace);

            var storedRow = await _repo.FindByTransactionHashAsync(transactionHash);

            if (storedRow == null)
                storedRow = await _repo.FindByAddressAndTransactionHashAsync(address, transactionHash);

            Assert.NotNull(storedRow);

            Assert.Equal(transactionHash, storedRow.TransactionHash);
            Assert.Equal(address, storedRow.Address);
            var structLogs = JArray.Parse(storedRow.StructLogs);
            Assert.NotNull(structLogs);
            Assert.Equal(2, structLogs.Count);
            Assert.Equal("log1", structLogs[0]);
            Assert.Equal("log2", structLogs[1]);
        }
    }
}
