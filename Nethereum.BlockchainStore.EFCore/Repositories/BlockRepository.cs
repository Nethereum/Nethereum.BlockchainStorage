using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Processors;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Block = Nethereum.BlockchainStore.Entities.Block;

namespace Nethereum.BlockchainStore.EFCore.Repositories
{
    public class BlockRepository : RepositoryBase, IBlockRepository
    {
        public BlockRepository(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        public async Task UpsertBlockAsync(BlockWithTransactionHashes source)
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
