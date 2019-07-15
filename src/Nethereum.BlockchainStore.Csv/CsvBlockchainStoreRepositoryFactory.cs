using Nethereum.BlockchainProcessing.ProgressRepositories;
using Nethereum.BlockchainProcessing.Storage.Repositories;
using Nethereum.BlockchainStore.Csv.Repositories;
using System;
using System.Collections.Generic;
using System.IO;

namespace Nethereum.BlockchainStore.Csv
{
    public class CsvBlockchainStoreRepositoryFactory : IBlockchainStoreRepositoryFactory, IBlockProgressRepositoryFactory
    {
        private readonly string _csvFolderPath;

        public CsvBlockchainStoreRepositoryFactory(string csvFolderPath)
        {
            this._csvFolderPath = csvFolderPath;

            if (!Directory.Exists(_csvFolderPath))
            {
                throw new ArgumentException($"Directory does not exist. '{_csvFolderPath}'");
            }
        }

        public IBlockProgressRepository CreateBlockProgressRepository()
        {
            return CreateRepository(() => new BlockProgressRepository(_csvFolderPath));
        }

        public IAddressTransactionRepository CreateAddressTransactionRepository()
        {
            return CreateRepository(() => new AddressTransactionRepository(_csvFolderPath));
        }

        public IBlockRepository CreateBlockRepository()
        {
            return CreateRepository(() => new BlockRepository(_csvFolderPath));
        }

        public IContractRepository CreateContractRepository()
        {
            return CreateRepository(() => new ContractRepository(_csvFolderPath));
        }

        public ITransactionLogRepository CreateTransactionLogRepository()
        {
            return CreateRepository(() => new TransactionLogRepository(_csvFolderPath));
        }

        public ITransactionRepository CreateTransactionRepository()
        {
            return CreateRepository(() => new TransactionRepository(_csvFolderPath));
        }

        public ITransactionVMStackRepository CreateTransactionVmStackRepository()
        {
            return CreateRepository(() => new TransactionVMStackRepository(_csvFolderPath));
        }

        private List<IDisposable> repositories = new List<IDisposable>();

        protected T CreateRepository<T>(Func<T> createRepoAction) where T : IDisposable
        {
            var repo = createRepoAction();
            repositories.Add(repo);
            return repo;
        }

        public IEnumerable<IDisposable> GetDisposableRepositories()
        {
            return repositories;
        }

        public void DisposeRepositories()
        {
            foreach (var repo in GetDisposableRepositories()) repo.Dispose();
        }
    }
}
