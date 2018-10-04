using Nethereum.BlockchainStore.Handlers;
using Nethereum.BlockchainStore.Processing;
using Nethereum.BlockchainStore.Processors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Repositories
{
    public class PersistenceStrategy: IBlockchainProcessingStrategy
    {   
        private readonly IContractRepository _contractRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly IWaitStrategy _waitStrategy;

        private readonly List<object> _repositories = new List<object>();

        public PersistenceStrategy(
            IBlockchainStoreRepositoryFactory repositoryFactory, 
            FilterContainer filters,
            IWaitStrategy waitStrategy = null,
            int maxRetries = 3,
            long minimumBlockNumber = 0)
        {
            MinimumBlockNumber = minimumBlockNumber;
            MaxRetries = maxRetries;
            Filters = filters;
            _waitStrategy = waitStrategy ?? new WaitStrategy();
            _blockRepository = repositoryFactory.CreateBlockRepository();
            _contractRepository = repositoryFactory.CreateContractRepository();

            var transactionRepository = repositoryFactory.CreateTransactionRepository();
            var addressTransactionRepository = repositoryFactory.CreateAddressTransactionRepository();
            var logRepository = repositoryFactory.CreateTransactionLogRepository();
            var vmStackRepository = repositoryFactory.CreateTransactionVmStackRepository();

            _repositories.Add(_blockRepository);
            _repositories.Add(_contractRepository);
            _repositories.Add(transactionRepository);
            _repositories.Add(addressTransactionRepository);
            _repositories.Add(logRepository);
            _repositories.Add(vmStackRepository);

            BlockHandler = new BlockHandler(_blockRepository);
            TransactionHandler = new TransactionHandler(transactionRepository, addressTransactionRepository);
            ContractHandler = new ContractHandler(_contractRepository);
            TransactionVmStackHandler = new TransactionVMStackHandler(vmStackRepository);
            TransactionLogHandler = new TransactionLogHandler(logRepository);

            _waitStrategy = new WaitStrategy();
        }

        public long MinimumBlockNumber { get; }
        public int MaxRetries { get; }

        public IBlockHandler BlockHandler { get; }
        public ITransactionHandler TransactionHandler { get; }
        public ITransactionLogHandler TransactionLogHandler { get; }
        public ITransactionVMStackHandler TransactionVmStackHandler { get; }
        public IContractHandler ContractHandler { get; }

        public FilterContainer Filters { get; }

        public async Task<long> GetLastBlockProcessedAsync()
        {
            return await _blockRepository.GetMaxBlockNumberAsync().ConfigureAwait(false);
        }

        public async Task FillContractCacheAsync()
        {
            await _contractRepository.FillCache().ConfigureAwait(false);
        }

        public async Task WaitForNextBlock(int retryNumber)
        {
            await _waitStrategy.Apply(retryNumber);
        }

        public async Task PauseFollowingAnError(int retryNumber)
        {
            await _waitStrategy.Apply(retryNumber);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var repo in _repositories)
                    {
                        if(repo is IDisposable disposableRepo)
                            disposableRepo.Dispose();
                    }

                    _repositories.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PersistenceStrategy() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
