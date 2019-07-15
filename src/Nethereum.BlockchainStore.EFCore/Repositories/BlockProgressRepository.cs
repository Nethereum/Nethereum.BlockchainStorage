using Microsoft.EntityFrameworkCore;
using Nethereum.BlockchainProcessing.ProgressRepositories;
using Nethereum.BlockchainProcessing.Storage.Entities;
using Nethereum.BlockchainProcessing.Storage.Entities.Mapping;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.EFCore.Repositories
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
                var blockRange = blockNumber.MapToStorageEntityForUpsert<BlockProgress>();

                context.BlockProgress.Add(blockRange);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
