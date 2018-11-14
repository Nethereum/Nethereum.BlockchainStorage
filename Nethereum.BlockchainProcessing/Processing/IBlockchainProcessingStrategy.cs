using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public interface IBlockchainProcessingStrategy: IDisposable
    {

        int MaxRetries { get; }
        long MinimumBlockNumber { get; }
        int MinimumBlockConfirmations { get; }

        Task WaitForNextBlock(int retryNumber);
        Task PauseFollowingAnError(int retryNumber);

        Task<long> GetLastBlockProcessedAsync();
        Task FillContractCacheAsync();

        Task ProcessBlockAsync(long blockNumber);
        Task<long> GetMaxBlockNumberAsync();
    }
}
