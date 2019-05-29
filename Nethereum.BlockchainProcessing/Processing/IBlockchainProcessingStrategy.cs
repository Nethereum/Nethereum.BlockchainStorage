using System;
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

        Task<ulong?> GetLastBlockProcessedAsync();
        Task FillContractCacheAsync();

        Task ProcessBlockAsync(ulong blockNumber);
        Task<ulong> GetMaxBlockNumberAsync();
    }
}
