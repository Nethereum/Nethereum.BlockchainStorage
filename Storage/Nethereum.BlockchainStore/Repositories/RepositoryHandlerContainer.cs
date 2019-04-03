using System;
using System.Collections.Generic;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainStore.Repositories.Handlers;

namespace Nethereum.BlockchainStore.Repositories
{
    public class RepositoryHandlerContext: IDisposable
    {
        private readonly List<object> _repositories = new List<object>();

        public IBlockRepository BlockRepository { get; }
        public IContractRepository ContractRepository { get; }
        public ITransactionRepository TransactionRepository { get; }
        public ITransactionLogRepository TransactionLogRepository { get; }
        public ITransactionVMStackRepository TransactionVmStackRepository { get; }
        public IAddressTransactionRepository AddressTransactionRepository { get; }

        public RepositoryHandlerContext(IBlockchainStoreRepositoryFactory repositoryFactory)
        {
            BlockRepository = repositoryFactory.CreateBlockRepository();
            ContractRepository = repositoryFactory.CreateContractRepository();
            TransactionRepository = repositoryFactory.CreateTransactionRepository();
            AddressTransactionRepository = repositoryFactory.CreateAddressTransactionRepository();
            TransactionLogRepository = repositoryFactory.CreateTransactionLogRepository();
            TransactionVmStackRepository = repositoryFactory.CreateTransactionVmStackRepository();

            _repositories.Add(BlockRepository);
            _repositories.Add(ContractRepository);
            _repositories.Add(TransactionLogRepository);
            _repositories.Add(AddressTransactionRepository);
            _repositories.Add(TransactionLogRepository);
            _repositories.Add(TransactionVmStackRepository);

            Handlers = new HandlerContainer
            {
                BlockHandler = new BlockRepositoryHandler(BlockRepository),
                ContractHandler = new ContractRepositoryHandler(ContractRepository),
                TransactionHandler = new TransactionRepositoryHandler(TransactionRepository),
                TransactionLogHandler = new TransactionLogRepositoryHandler(TransactionLogRepository),
                TransactionVmStackHandler = new TransactionVMStackRepositoryHandler(TransactionVmStackRepository)
            };
        }

        public HandlerContainer Handlers { get; private set; }

        #region IDisposable Support
        private bool disposed = false; // To detect redundant calls
        

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    foreach (var repo in _repositories)
                    {
                        if(repo is IDisposable disposableRepo)
                            disposableRepo.Dispose();
                    }

                    _repositories.Clear();

                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposed = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RepositoryHandlerContainer() {
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
