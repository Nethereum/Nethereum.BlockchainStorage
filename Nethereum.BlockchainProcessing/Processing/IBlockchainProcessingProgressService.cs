using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public interface IBlockchainProcessingProgressService
    {
        Task UpsertBlockNumberProcessedTo(ulong blockNumber);
        Task<BlockRange?> GetNextBlockRangeToProcess(uint maxBlocksToProcessInBatch);
    }
}
