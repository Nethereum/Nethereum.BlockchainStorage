using Microsoft.EntityFrameworkCore;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using System.Threading.Tasks;
using Block = Nethereum.BlockchainStore.Entities.Block;

namespace Nethereum.BlockchainStore.EFCore.Repositories
{
    public class BlockRepository : RepositoryBase, IBlockRepository
    {
        public BlockRepository(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        public async Task<IBlockView> FindByBlockNumberAsync(HexBigInteger blockNumber)
        {
            using (var context = _contextFactory.CreateContext())
            {
                return await context.Blocks.FindByBlockNumberAsync(blockNumber).ConfigureAwait(false);
            }
        }

        public async Task<ulong> GetMaxBlockNumberAsync()
        {
            using (var context = _contextFactory.CreateContext())
            {
                var max = await context.Blocks.MaxAsync(b => b.BlockNumber).ConfigureAwait(false);
                return string.IsNullOrEmpty(max) ? 0 : ulong.Parse(max);
            }
        }

        public async Task UpsertBlockAsync(Nethereum.RPC.Eth.DTOs.Block source)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var block = await context.Blocks.FindByBlockNumberAsync(source.Number).ConfigureAwait(false) ?? new Block();

                block.Map(source);

                block.UpdateRowDates();

                if (block.IsNew())
                    context.Blocks.Add(block);
                else
                    context.Blocks.Update(block);

                await context.SaveChangesAsync().ConfigureAwait(false) ;
            }
        }
    }
}
