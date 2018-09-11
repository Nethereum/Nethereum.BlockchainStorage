using Nethereum.BlockchainStore.EF.Repositories;
using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Repositories;

namespace Nethereum.BlockchainStore.EF
{
    public class BlockchainStoreRepositoryFactory : IBlockchainStoreEntityRepositoryFactory
    {
        private readonly IBlockchainDbContextFactory _blockchainDbContextFactory;

        public BlockchainStoreRepositoryFactory(IBlockchainDbContextFactory contextFactory)
        {
            _blockchainDbContextFactory = contextFactory;
        }

        public IEntityBlockRepository CreateEntityBlockRepository() => new BlockRepository(_blockchainDbContextFactory);
        public IAddressTransactionRepository CreateAddressTransactionRepository() => new AddressTransactionRepository(_blockchainDbContextFactory);
        public IEntityContractRepository CreateEntityContractRepository() => new ContractRepository(_blockchainDbContextFactory);
        public IEntityTransactionLogRepository CreateEntityTransactionLogRepository() => new TransactionLogRepository(_blockchainDbContextFactory);
        public IEntityTransactionVMStackRepository CreateEntityTransactionVmStackRepository() => new TransactionVMStackRepository(_blockchainDbContextFactory);
        public IEntityTransactionRepository CreateEntityTransactionRepository() => new TransactionRepository(_blockchainDbContextFactory);

        public IBlockRepository CreateBlockRepository() => CreateEntityBlockRepository();
        public IContractRepository CreateContractRepository() => CreateEntityContractRepository();
        public ITransactionLogRepository CreateTransactionLogRepository() => CreateEntityTransactionLogRepository();
        public ITransactionVMStackRepository CreateTransactionVmStackRepository() => CreateEntityTransactionVmStackRepository();
        public ITransactionRepository CreateTransactionRepository() => CreateEntityTransactionRepository();

    }
}
