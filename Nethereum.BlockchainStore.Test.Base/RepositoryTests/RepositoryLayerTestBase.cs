using System.Threading.Tasks;
using Nethereum.BlockchainStore.Repositories;
using Xunit;

namespace Nethereum.BlockchainStore.Test.Base.RepositoryTests
{
    public abstract class RepositoryLayerTestBase
    {
        private readonly IBlockchainStoreEntityRepositoryFactory _repositoryFactory;

        protected RepositoryLayerTestBase(IBlockchainStoreEntityRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        [Fact]
        public virtual async Task BlockRepositoryTests()
        {
            await new BlockRepositoryTests(_repositoryFactory.CreateEntityBlockRepository()).RunAsync();
        } 

        [Fact]
        public virtual async Task ContractRepositoryTests()
        {
            await new ContractRepositoryTests(_repositoryFactory.CreateEntityContractRepository()).RunAsync();
        } 
    
        [Fact]
        public virtual async Task TransactionLogRepositoryTests()
        {
            await new TransactionLogRepositoryTests( _repositoryFactory.CreateEntityTransactionLogRepository()).RunAsync();
        } 

        [Fact]
        public virtual async Task TransactionLogVMStackTests()
        {
            await new TransactionVMStackRepositoryTests(_repositoryFactory.CreateEntityTransactionVmStackRepository()).RunAsync();
        } 
    }
}
