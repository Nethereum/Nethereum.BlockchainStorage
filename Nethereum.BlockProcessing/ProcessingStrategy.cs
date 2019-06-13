using Nethereum.BlockchainProcessing.Processors;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public class ProcessingStrategy : IBlockchainProcessingStrategy
    {
        private static readonly Task<BigInteger?> TaskReturnNull = Task.FromResult((BigInteger?)null);
        protected readonly IBlockProcessor BlockProcessor;

        public ProcessingStrategy(IBlockProcessor blockProcessor)
        {
            BlockProcessor = blockProcessor;
        }

        public virtual IWaitStrategy WaitStrategy { get; set; } = new WaitStrategy();

        public virtual uint MaxRetries { get; set; } = 3;
        public virtual ulong MinimumBlockNumber { get; set; } = 0;
        public virtual uint MinimumBlockConfirmations { get; set; } = 0;

        public virtual Task FillContractCacheAsync() { return Task.CompletedTask; }

        public virtual Task<BigInteger?> GetLastBlockProcessedAsync() => TaskReturnNull;
        public virtual Task PauseFollowingAnError(uint retryNumber) => WaitStrategy.Apply(retryNumber);
        public virtual Task WaitForNextBlock(uint retryNumber) => WaitStrategy.Apply(retryNumber);
        public virtual Task ProcessBlockAsync(BigInteger blockNumber) => BlockProcessor.ProcessBlockAsync(blockNumber);
        public virtual Task<BigInteger> GetMaxBlockNumberAsync() => BlockProcessor.GetMaxBlockNumberAsync();
    }
}
