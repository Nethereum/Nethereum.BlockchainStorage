using Nethereum.BlockchainStore.EFCore.Tests.Base.RepositoryTests;

namespace Nethereum.BlockchainStore.EFCore.Tests.SqlServer.RepositoryTests
{
    public class TransactionVMStackRepositoryTests: TransactionVMStackRepositoryBaseTests
    {
        public TransactionVMStackRepositoryTests():base(new TestSqlServerBlockchainDbContextFactory())
        {
        }
    }
}
