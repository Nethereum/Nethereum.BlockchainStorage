using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.BlockchainProcessing.ProgressRepositories;
using System.Data.Entity;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.EF.Repositories
{
    public class BlockProgressRepository : RepositoryBase, IBlockProgressRepository
    {
        public BlockProgressRepository(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        public async Task<BigInteger?> GetLastBlockNumberProcessedAsync()
        {
            using (var context = _contextFactory.CreateContext())
            {
                var max = await context.BlockProgress.MaxAsync(b => b.LastBlockProcessed).ConfigureAwait(false);
                return string.IsNullOrEmpty(max) ? (BigInteger?)null : BigInteger.Parse(max);
            }
        }

        public async Task UpsertProgressAsync(BigInteger blockNumber)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var blockEntity = blockNumber.MapToStorageEntityForUpsert();

                context.BlockProgress.Add(blockEntity);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
