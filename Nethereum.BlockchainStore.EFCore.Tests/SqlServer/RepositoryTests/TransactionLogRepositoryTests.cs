using Nethereum.BlockchainStore.EFCore.Tests.Base.RepositoryTests;

namespace Nethereum.BlockchainStore.EFCore.Tests.SqlServer.RepositoryTests
{
    public class TransactionLogRepositoryTests: TransactionLogRepositoryBaseTests
    {
        public TransactionLogRepositoryTests():base(new TestSqlServerBlockchainDbContextFactory())
        {
        }
    }
}
