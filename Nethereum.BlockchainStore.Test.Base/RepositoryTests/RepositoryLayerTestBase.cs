using System;
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
            var repo = _repositoryFactory.CreateBlockRepository();
            try
            {
                await new BlockRepositoryTests(repo).RunAsync();
            }
            finally
            {
                if(repo is IDisposable dispRep)
                    dispRep.Dispose();
            }
        } 

        [Fact]
        public virtual async Task ContractRepositoryTests()
        {
            var repo = _repositoryFactory.CreateContractRepository();
            try
            {
                await new ContractRepositoryTests(repo).RunAsync();
            }
            finally
            {
                if(repo is IDisposable dispRep)
                    dispRep.Dispose();
            }
        } 
    
        [Fact]
        public virtual async Task TransactionRepositoryTests()
        {
            var repo = _repositoryFactory.CreateTransactionRepository();
            try
            {
                await new TransactionRepositoryTests(repo).RunAsync();
            }
            finally
            {
                if(repo is IDisposable dispRep)
                    dispRep.Dispose();
            }
        } 

        [Fact]
        public virtual async Task TransactionLogRepositoryTests()
        {
            var repo = _repositoryFactory.CreateTransactionLogRepository();
            try
            {
                await new TransactionLogRepositoryTests(repo).RunAsync();
            }
            finally
            {
                if(repo is IDisposable dispRep)
                    dispRep.Dispose();
            }
        } 

        [Fact]
        public virtual async Task TransactionLogVMStackTests()
        {
            var repo = _repositoryFactory.CreateTransactionVmStackRepository();
            try
            {
                await new TransactionVMStackRepositoryTests(repo).RunAsync();
            }
            finally
            {
                if(repo is IDisposable dispRep)
                    dispRep.Dispose();
            }
        } 
    }
}
