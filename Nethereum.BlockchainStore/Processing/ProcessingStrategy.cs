using Nethereum.BlockchainStore.Handlers;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processing
{
    public class ProcessingStrategy : IBlockchainProcessingStrategy
    {
        public virtual FilterContainer Filters { get; set; } = null;
        public virtual IBlockHandler BlockHandler { get; set;} = new NullBlockHandler();
        public virtual ITransactionHandler TransactionHandler { get; set;} = new NullTransactionHandler();
        public virtual ITransactionLogHandler TransactionLogHandler { get; set;} = new NullTransactionLogHandler();
        public virtual ITransactionVMStackHandler TransactionVmStackHandler { get; set;} = new NullTransactionVMStackHandler();
        public virtual IContractHandler ContractHandler { get; set;} = new NullContractHandler();
        public virtual int MaxRetries => 3;
        public virtual long MinimumBlockNumber => 0;

        public int MinimumBlockConfirmations { get; set; } = 0;

        protected virtual IWaitStrategy WaitStrategy { get; set; } = new WaitStrategy();

        public virtual void Dispose() { }

        public virtual Task FillContractCacheAsync() { return Task.CompletedTask; }

        public virtual Task<long> GetLastBlockProcessedAsync() => Task.FromResult((long)0);

        public virtual Task PauseFollowingAnError(int retryNumber) => WaitStrategy.Apply(retryNumber);

        public virtual Task WaitForNextBlock(int retryNumber) => WaitStrategy.Apply(retryNumber);
    }
}
