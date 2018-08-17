using Nethereum.BlockchainStore.EFCore.Tests.Base.RepositoryTests;

namespace Nethereum.BlockchainStore.EFCore.Tests.Sqlite.RepositoryTests
{
    public class TransactionRepositoryTests: TransactionRepositoryBaseTests
    {
        public TransactionRepositoryTests():base(new TestSqliteDbContextFactory())
        {
        }
    }
}
