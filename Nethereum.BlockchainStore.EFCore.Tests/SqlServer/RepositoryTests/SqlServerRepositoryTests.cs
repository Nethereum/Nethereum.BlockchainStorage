using Nethereum.BlockchainStore.Test.Base.RepositoryTests;

namespace Nethereum.BlockchainStore.EFCore.Tests.SqlServer.RepositoryTests
{
    public class SqlServerRepositoryTests : RepositoryLayerTestBase
    {
        public SqlServerRepositoryTests() : base(new BlockchainStoreRepositoryFactory(new TestSqlServerBlockchainDbContextFactory()))
        {
        }
    }
}
