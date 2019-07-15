using Nethereum.BlockchainStore.AzureTables.Repositories;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.AzureTables.Tests.RepositoryTests
{

    [Collection("AzureTablesFixture")]
    public class BlockProgressRepositoryTests
    {
        private readonly BlockProgressRepository _repo;
        private readonly BlockProgressRepository _repoCopy;

        public BlockProgressRepositoryTests(AzureTablesFixture fixture)
        {
            _repo = fixture.Factory.CreateBlockProgressRepository() as BlockProgressRepository;
            _repoCopy = fixture.Factory.CreateBlockProgressRepository() as BlockProgressRepository;
        }

        [Fact]
        public async Task Run()
        {
            // these are run as part of one test to avoid delays in azure table setup and tear down
            // the order is important
            await ShouldReturnNullWhenEmpty();
            await ShouldRetrievePreviousStateOnInitialisation();
            await StoresAndRetrievesLatestBlockNumber();
        }

        private async Task ShouldReturnNullWhenEmpty()
        {
            var lastBlockNumber = await _repo.GetLastBlockNumberProcessedAsync();
            Assert.Null(lastBlockNumber);
        } 

        private async Task ShouldRetrievePreviousStateOnInitialisation()
        {
            await _repo.UpsertProgressAsync(101); // upsert on on the first repo

            //retrieve from the other repo - which should read from table storage and give the same result
            var lastBlockNumber = await _repoCopy.GetLastBlockNumberProcessedAsync();
            Assert.Equal((ulong)101, lastBlockNumber);
        } 

        private async Task StoresAndRetrievesLatestBlockNumber()
        {
            await _repo.UpsertProgressAsync(99);

            var lastBlockNumber = await _repo.GetLastBlockNumberProcessedAsync();
            Assert.Equal((ulong)99, lastBlockNumber);
        } 
    }
}
