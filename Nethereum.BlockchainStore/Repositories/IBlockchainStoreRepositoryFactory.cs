using Nethereum.BlockchainStore.Processors;

namespace Nethereum.BlockchainStore.Repositories
{
    public interface IBlockchainStoreRepositoryFactory
    {
        IAddressTransactionRepository CreateAddressTransactionRepository();
        IBlockRepository CreatBlockRepository();
        IContractRepository CreateContractRepository();
        ITransactionLogRepository CreateTransactionLogRepository();
        ITransactionRepository CreatetTransactionRepository();
        ITransactionVMStackRepository CreateTransactionVmStackRepository();
    }
}
