using Nethereum.BlockchainStore.Processors;

namespace Nethereum.BlockchainStore.Repositories
{
    public interface IBlockchainStoreRepositoryFactory
    {
        IAddressTransactionRepository CreateAddressTransactionRepository();
        IBlockRepository CreateBlockRepository();
        IContractRepository CreateContractRepository();
        ITransactionLogRepository CreateTransactionLogRepository();
        ITransactionRepository CreateTransactionRepository();
        ITransactionVMStackRepository CreateTransactionVmStackRepository();
    }

    public interface IBlockchainStoreEntityRepositoryFactory: IBlockchainStoreRepositoryFactory
    {
        IEntityBlockRepository CreateEntityBlockRepository();
        IEntityContractRepository CreateEntityContractRepository();
        IEntityTransactionLogRepository CreateEntityTransactionLogRepository();
        IEntityTransactionRepository CreateEntityTransactionRepository();
        IEntityTransactionVMStackRepository CreateEntityTransactionVmStackRepository();
    }
}
