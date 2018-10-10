using System.Threading.Tasks;
using Nethereum.BlockchainStore.Handlers;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Repositories
{
    public class BlockHandler : IBlockHandler
    {
        private readonly IBlockRepository _blockRepository;

        public BlockHandler(IBlockRepository blockRepository)
        {
            this._blockRepository = blockRepository;
        }

        public async Task HandleAsync(Block block)
        {
            await _blockRepository.UpsertBlockAsync(block);
        }
    }
}
