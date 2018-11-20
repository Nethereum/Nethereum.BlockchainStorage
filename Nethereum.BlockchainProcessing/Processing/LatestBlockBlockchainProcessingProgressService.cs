using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Web3Abstractions;

namespace Nethereum.BlockchainProcessing.Processing
{
    public class LatestBlockBlockchainProcessingProgressService : BlockchainProcessingProgressService
    {
        private readonly IWeb3Wrapper _web3;

        public LatestBlockBlockchainProcessingProgressService(
            IWeb3Wrapper web3, 
            ulong defaultStartingBlockNumber, 
            IBlockProcessProgressRepository blockProcessProgressRepository) : 
            base(
            defaultStartingBlockNumber, 
            blockProcessProgressRepository)
        {
            _web3 = web3;
        }

        public override async Task<ulong> GetBlockNumberToProcessTo()
        {
            return await _web3.GetMaxBlockNumberAsync().ConfigureAwait(false);
        }
    }
}