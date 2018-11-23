using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public interface IBlockProgressService
    {
        Task SaveLastBlockProcessedAsync(ulong blockNumber);
        Task<BlockRange?> GetNextBlockRangeToProcessAsync(uint maxBlocksToProcessInBatch);
    }
}
