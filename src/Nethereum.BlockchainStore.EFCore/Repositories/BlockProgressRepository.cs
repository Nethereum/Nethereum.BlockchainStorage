using Microsoft.EntityFrameworkCore;
using Nethereum.BlockchainProcessing.ProgressRepositories;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using System.Numerics;
using System.Threading.Tasks;
using System;

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
            try
            {
                using (var context = _contextFactory.CreateContext())
                {
                    var blockRange = blockNumber.MapToStorageEntityForUpsert<BlockProgress>();
                    blockRange.LastBlockProcessed = blockNumber.ToString().PadLeft(ColumnLengths.BigIntegerLength, '0');
                    context.BlockProgress.Add(blockRange);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("String or binary data would be truncated") ?? false)
            {
                throw new DbUpdateException(
                    $"{nameof(BlockProgressRepository)} Data Truncation Rrror. Ensure that the LastBlockProcessed column length is {ColumnLengths.BigIntegerLength}."
                    , ex);
            }
        }
    }
}
