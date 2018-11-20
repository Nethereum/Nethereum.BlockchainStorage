using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public class PreDefinedRangeBlockchainProcessingProgressService : BlockchainProcessingProgressService
    {
        public PreDefinedRangeBlockchainProcessingProgressService(
            ulong blockNumberFrom, 
            ulong blockNumberTo,
            IBlockProcessProgressRepository blockProcessProgressRepository) : 
            base(
                blockNumberFrom, 
                blockProcessProgressRepository)
        {
            BlockNumberTo = blockNumberTo;
        }

        public ulong BlockNumberTo { get; }

        public override Task<ulong> GetBlockNumberToProcessTo()
        {
            return Task.FromResult(BlockNumberTo);
        }
    }
}