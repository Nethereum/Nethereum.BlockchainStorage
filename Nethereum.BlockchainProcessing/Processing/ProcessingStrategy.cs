using Nethereum.BlockchainProcessing.Processors;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public class ProcessingStrategy : IBlockchainProcessingStrategy
    {
        private static readonly Task<ulong> TaskReturningZero = Task.FromResult((ulong)0);
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

        public virtual Task<ulong> GetLastBlockProcessedAsync() => TaskReturningZero;
        public virtual Task PauseFollowingAnError(uint retryNumber) => WaitStrategy.Apply(retryNumber);
        public virtual Task WaitForNextBlock(uint retryNumber) => WaitStrategy.Apply(retryNumber);
        public virtual Task ProcessBlockAsync(ulong blockNumber) => BlockProcessor.ProcessBlockAsync(blockNumber);
        public virtual Task<ulong> GetMaxBlockNumberAsync() => BlockProcessor.GetMaxBlockNumberAsync();
    }
}
