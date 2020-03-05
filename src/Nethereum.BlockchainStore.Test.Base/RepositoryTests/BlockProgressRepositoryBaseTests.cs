using Nethereum.BlockchainProcessing.ProgressRepositories;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Test.Base.RepositoryTests
{
    public class BlockProgressRepositoryTests: IRepositoryTest
    {
        private readonly IBlockProgressRepository _repo;

        public BlockProgressRepositoryTests(IBlockProgressRepository repo)
        {
            _repo = repo;
        }

        public async Task RunAsync()
        {
            // prove numeric sorting
            Assert.Null(await _repo.GetLastBlockNumberProcessedAsync());
            await _repo.UpsertProgressAsync(997);
            Assert.Equal(997, await _repo.GetLastBlockNumberProcessedAsync());
            await _repo.UpsertProgressAsync(2261);
            Assert.Equal(2261, await _repo.GetLastBlockNumberProcessedAsync());
        }
    }
}
