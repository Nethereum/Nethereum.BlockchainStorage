using Nethereum.BlockchainStore.Csv.Repositories;
using Nethereum.BlockchainStore.Repositories;
using System;
using System.IO;

namespace Nethereum.BlockchainStore.Csv
{
    public class CsvBlockchainStoreRepositoryFactory : IBlockchainStoreRepositoryFactory
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

        public IAddressTransactionRepository CreateAddressTransactionRepository()
        {
            return new AddressTransactionRepository(_csvFolderPath);
        }

        public IBlockRepository CreateBlockRepository()
        {
            return new BlockRepository(_csvFolderPath);
        }

        public IContractRepository CreateContractRepository()
        {
            return new ContractRepository(_csvFolderPath);
        }

        public ITransactionLogRepository CreateTransactionLogRepository()
        {
            return new TransactionLogRepository(_csvFolderPath);
        }

        public ITransactionRepository CreateTransactionRepository()
        {
            return new TransactionRepository(_csvFolderPath);
        }

        public ITransactionVMStackRepository CreateTransactionVmStackRepository()
        {
            return new TransactionVMStackRepository(_csvFolderPath);
        }
    }
}
