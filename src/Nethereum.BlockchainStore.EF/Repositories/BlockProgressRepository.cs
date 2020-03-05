using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.BlockchainProcessing.ProgressRepositories;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
            try
            {
                using (var context = _contextFactory.CreateContext())
                {
                    var blockEntity = blockNumber.MapToStorageEntityForUpsert();
                    blockEntity.LastBlockProcessed = blockEntity.LastBlockProcessed.PadLeft(ColumnLengths.BigIntegerLength, '0');
                    context.BlockProgress.Add(blockEntity);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch(DbUpdateException ex) 
            when (ex.InnerException?.InnerException.Message.Contains("String or binary data would be truncated") ?? false)
            {
                throw new DbUpdateException(
                    $"{nameof(BlockProgressRepository)} Data Truncation Rrror. Ensure that the LastBlockProcessed column length is {ColumnLengths.BigIntegerLength}."
                    , ex);
            }
        }
    }
}
