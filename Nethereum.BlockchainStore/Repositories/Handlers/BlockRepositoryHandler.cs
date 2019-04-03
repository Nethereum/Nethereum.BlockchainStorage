using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Repositories.Handlers
{
    public class BlockRepositoryHandler : IBlockHandler
    {
        private readonly IBlockRepository _blockRepository;

        public BlockRepositoryHandler(IBlockRepository blockRepository)
        {
            _blockRepository = blockRepository;
        }

        public async Task HandleAsync(Block block)
        {
            await _blockRepository.UpsertBlockAsync(block);
        }
    }
}
