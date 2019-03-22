using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.BlockchainProxy;

namespace Nethereum.BlockchainProcessing.Processing
{
    public class BlockProgressService : BlockProgressServiceBase
    {
        private readonly IBlockchainProxyService _web3;

        public uint MinimumBlockConfirmations { get; }

        public BlockProgressService(
            IBlockchainProxyService blockchainProxyService, 
            ulong defaultStartingBlockNumber, 
            IBlockProgressRepository blockProgressRepository,
            uint minimumBlockConfirmations = 0) : 
            base(
            defaultStartingBlockNumber, 
            blockProgressRepository)
        {
            _web3 = blockchainProxyService;
            MinimumBlockConfirmations = minimumBlockConfirmations;
        }

        public override async Task<ulong> GetBlockNumberToProcessTo()
        {
            return await _web3.GetMaxBlockNumberAsync()
                       .ConfigureAwait(false) - MinimumBlockConfirmations;
        }
    }
}