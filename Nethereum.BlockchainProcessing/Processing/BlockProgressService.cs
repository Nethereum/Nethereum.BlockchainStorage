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
            ulong? defaultStartingBlockNumber, 
            IBlockProgressRepository blockProgressRepository,
            uint? minimumBlockConfirmations = null) : 
            base(
            defaultStartingBlockNumber, 
            blockProgressRepository)
        {
            _web3 = blockchainProxyService;
            MinimumBlockConfirmations = minimumBlockConfirmations ?? 0;
        }

        protected override Task<ulong> GetMinBlockNumber() => GetCurrentBlockNumberLessMinimumConfirmations();

        public override Task<ulong> GetBlockNumberToProcessTo() => GetCurrentBlockNumberLessMinimumConfirmations();

        private  async Task<ulong> GetCurrentBlockNumberLessMinimumConfirmations()
        {
            return await _web3.GetMaxBlockNumberAsync()
                       .ConfigureAwait(false) - MinimumBlockConfirmations;
        }
    }
}