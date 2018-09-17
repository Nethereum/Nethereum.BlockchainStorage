using System.Threading.Tasks;
using Nethereum.BlockchainStore.Repositories;
using Xunit;

namespace Nethereum.BlockchainStore.Test.Base.RepositoryTests
{
    public abstract class RepositoryLayerTestBase
    {
        private readonly IBlockchainStoreRepositoryFactory _repositoryFactory;

        protected RepositoryLayerTestBase(IBlockchainStoreRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        [Fact]
        public virtual async Task BlockRepositoryTests()
        {
            await new BlockRepositoryTests(_repositoryFactory.CreateBlockRepository()).RunAsync();
        } 

        [Fact]
        public virtual async Task ContractRepositoryTests()
        {
            await new ContractRepositoryTests(_repositoryFactory.CreateContractRepository()).RunAsync();
        } 
    
        [Fact]
        public virtual async Task TransactionRepositoryTests()
        {
            await new TransactionRepositoryTests( _repositoryFactory.CreateTransactionRepository()).RunAsync();
        } 

        [Fact]
        public virtual async Task TransactionLogRepositoryTests()
        {
            await new TransactionLogRepositoryTests( _repositoryFactory.CreateTransactionLogRepository()).RunAsync();
        } 

        [Fact]
        public virtual async Task TransactionLogVMStackTests()
        {
            await new TransactionVMStackRepositoryTests(_repositoryFactory.CreateTransactionVmStackRepository()).RunAsync();
        } 
    }
}
