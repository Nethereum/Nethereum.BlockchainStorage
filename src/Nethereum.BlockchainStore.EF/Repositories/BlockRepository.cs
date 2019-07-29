using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.Hex.HexTypes;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.EF.Repositories
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

        public async Task UpsertBlockAsync(Nethereum.RPC.Eth.DTOs.Block source)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var block = await context.Blocks.FindByBlockNumberAsync(source.Number).ConfigureAwait(false) ?? new Block();

                block.MapToStorageEntityForUpsert(source);
                context.Blocks.AddOrUpdate(block);

                await context.SaveChangesAsync().ConfigureAwait(false) ;
            }
        }
    }
}
