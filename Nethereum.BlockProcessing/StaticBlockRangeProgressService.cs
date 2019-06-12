using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public class StaticBlockRangeProgressService : BlockProgressServiceBase
    {
        public StaticBlockRangeProgressService(
            ulong blockNumberFrom, 
            ulong blockNumberTo,
            IBlockProgressRepository blockProgressRepository) : 
            base(
                blockNumberFrom, 
                blockProgressRepository)
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