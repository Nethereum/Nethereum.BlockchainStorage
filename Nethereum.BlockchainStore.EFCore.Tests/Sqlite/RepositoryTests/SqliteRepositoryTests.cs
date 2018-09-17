using Nethereum.BlockchainStore.Test.Base.RepositoryTests;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Nethereum.BlockchainStore.EFCore.Tests.Sqlite.RepositoryTests
{
    public class SqliteTestFixture : IDisposable
    {
        public SqliteTestFixture()
        {
            DeleteDatabase();
            Factory = new TestSqliteDbContextFactory();
        }

        public TestSqliteDbContextFactory Factory { get; }

        public void Dispose()
        {
            DeleteDatabase();
        }

        private void DeleteDatabase()
        {
            try
            {
                var configValues =
                    TestSqliteDbContextFactory.ConnectionString.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);

                var dataSource = configValues.First(p =>
                    p.StartsWith("Data Source", StringComparison.InvariantCultureIgnoreCase));

                var path = dataSource.Split('=')[1].Trim();
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
                //ignore
            }
        }
    }

    [CollectionDefinition("SqliteTestFixture")]
    public class SqlLiteFixtureCollection : ICollectionFixture<SqliteTestFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [Collection("SqliteTestFixture")]
    public class SqliteRepositoryTests : RepositoryLayerTestBase
    {
        public SqliteRepositoryTests(SqliteTestFixture fixture) : base(new BlockchainStoreRepositoryFactory(fixture.Factory))
        {
        }
    }
}
