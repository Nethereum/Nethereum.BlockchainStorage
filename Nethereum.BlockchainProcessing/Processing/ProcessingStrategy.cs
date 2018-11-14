using Nethereum.BlockchainProcessing.Processors;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public class ProcessingStrategy : IBlockchainProcessingStrategy
    {
        private static readonly Task<long> TaskReturningZero = Task.FromResult((long)0);
        protected readonly IBlockProcessor BlockProcessor;

        public ProcessingStrategy(IBlockProcessor blockProcessor)
        {
            BlockProcessor = blockProcessor;
        }

        public virtual IWaitStrategy WaitStrategy { get; set; } = new WaitStrategy();

        public virtual int MaxRetries { get; set; } = 3;
        public virtual long MinimumBlockNumber { get; set; } = 0;
        public virtual int MinimumBlockConfirmations { get; set; } = 0;

        public virtual void Dispose() { }

        public virtual Task FillContractCacheAsync() { return Task.CompletedTask; }

        public virtual Task<long> GetLastBlockProcessedAsync() => TaskReturningZero;
        public virtual Task PauseFollowingAnError(int retryNumber) => WaitStrategy.Apply(retryNumber);
        public virtual Task WaitForNextBlock(int retryNumber) => WaitStrategy.Apply(retryNumber);
        public virtual Task ProcessBlockAsync(long blockNumber) => BlockProcessor.ProcessBlockAsync(blockNumber);
        public virtual Task<long> GetMaxBlockNumberAsync() => BlockProcessor.GetMaxBlockNumberAsync();
    }
}
