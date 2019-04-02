using Nethereum.BlockchainStore.Test.Base.RepositoryTests;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;

namespace Nethereum.BlockchainStore.EF.Tests.Sqlite.RepositoryTests
{
    public class SqliteTestFixture : IDisposable
    {
        public SqliteTestFixture()
        {
            Factory = new TestSqliteContextFactory();
            DeleteDatabase();
        }

        public TestSqliteContextFactory Factory { get; }

        public void Dispose()
        {
            DeleteDatabase();
        }

        private void DeleteDatabase(int attemptCount = 0)
        {
            try
            {
                attemptCount++;

                var db = Factory.CreateContext().Database;

                var configValues =
                    db.Connection.ConnectionString.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);

                var dataSource = configValues.First(p =>
                    p.StartsWith("Data Source", StringComparison.OrdinalIgnoreCase));

                var path = dataSource.Split('=')[1].Trim();
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
                if (attemptCount < 3)
                {
                    Thread.Sleep(1000);
                    DeleteDatabase(attemptCount);
                }

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
        public SqliteRepositoryTests(SqliteTestFixture fixture): base(new BlockchainStoreRepositoryFactory(fixture.Factory))
        {
        }

        
    }
}
