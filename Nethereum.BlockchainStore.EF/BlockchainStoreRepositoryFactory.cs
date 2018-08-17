﻿using Nethereum.BlockchainStore.EF.Repositories;
using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Repositories;

namespace Nethereum.BlockchainStore.EF
{
    public class BlockchainStoreRepositoryFactory : IBlockchainStoreRepositoryFactory
    {
        private readonly IBlockchainDbContextFactory _blockchainDbContextFactory;

        public BlockchainStoreRepositoryFactory(IBlockchainDbContextFactory contextFactory)
        {
            _blockchainDbContextFactory = contextFactory;
        }

        public IBlockRepository CreatBlockRepository() => new BlockRepository(_blockchainDbContextFactory);
        public IAddressTransactionRepository CreateAddressTransactionRepository() => new AddressTransactionRepository(_blockchainDbContextFactory);
        public IContractRepository CreateContractRepository() => new ContractRepository(_blockchainDbContextFactory);
        public ITransactionLogRepository CreateTransactionLogRepository() => new TransactionLogRepository(_blockchainDbContextFactory);
        public ITransactionVMStackRepository CreateTransactionVmStackRepository() => new TransactionVMStackRepository(_blockchainDbContextFactory);
        public ITransactionRepository CreatetTransactionRepository() => new TransactionRepository(_blockchainDbContextFactory);
    }
}