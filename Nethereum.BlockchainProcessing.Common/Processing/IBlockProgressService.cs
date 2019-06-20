using Nethereum.Contracts;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public interface IBlockProgressService
    {
        Task SaveLastBlockProcessedAsync(BigInteger blockNumber);
        Task<BlockRange?> GetNextBlockRangeToProcessAsync(uint maxBlocksToProcessInBatch);
    }
}
