using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public interface IBlockchainProcessingStrategy
    {
        uint MaxRetries { get; }
        ulong MinimumBlockNumber { get; }
        uint MinimumBlockConfirmations { get; }

        Task WaitForNextBlock(uint retryNumber);
        Task PauseFollowingAnError(uint retryNumber);

        Task<BigInteger?> GetLastBlockProcessedAsync();
        Task FillContractCacheAsync();

        Task ProcessBlockAsync(BigInteger blockNumber);
        Task<BigInteger> GetMaxBlockNumberAsync();
    }
}
