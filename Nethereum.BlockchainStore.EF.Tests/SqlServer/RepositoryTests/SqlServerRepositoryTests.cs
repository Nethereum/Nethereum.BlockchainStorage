using Nethereum.BlockchainStore.Test.Base.RepositoryTests;

namespace Nethereum.BlockchainStore.EF.Tests.SqlServer.RepositoryTests
{
    public class SqlServerRepositoryTests : RepositoryLayerTestBase
    {
        public SqlServerRepositoryTests() : base(new BlockchainStoreRepositoryFactory(new TestSqlServerContextFactory()))
        {
        }
    }
}
