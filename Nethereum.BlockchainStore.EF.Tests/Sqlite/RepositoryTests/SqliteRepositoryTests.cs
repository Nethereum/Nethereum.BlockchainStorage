using Nethereum.BlockchainStore.Test.Base.RepositoryTests;

namespace Nethereum.BlockchainStore.EF.Tests.Sqlite.RepositoryTests
{
    public class SqliteRepositoryTests : RepositoryLayerTestBase
    {
        public SqliteRepositoryTests(): base(new BlockchainStoreRepositoryFactory(new TestSqliteContextFactory()))
        {
        }

        
    }
}
