using Nethereum.BlockchainStore.Test.Base.RepositoryTests;
using System;
using Xunit;

namespace Nethereum.BlockchainStore.EF.Tests.SqlServer.RepositoryTests
{
    public class SqlServerTestFixture : IDisposable
    {
        public SqlServerTestFixture()
        {
            Factory = new TestSqlServerContextFactory();
            EmptyDatabase();
        }

        public TestSqlServerContextFactory Factory { get; }

        public void Dispose()
        {
            EmptyDatabase();
        }

        private void EmptyDatabase()
        {
            try
            {
                var ctx = Factory.CreateContext();
                ctx.Database.ExecuteSqlCommand("TRUNCATE TABLE Blocks");
                ctx.Database.ExecuteSqlCommand("TRUNCATE TABLE Contracts");
                ctx.Database.ExecuteSqlCommand("TRUNCATE TABLE AddressTransactions");
                ctx.Database.ExecuteSqlCommand("TRUNCATE TABLE Transactions");
                ctx.Database.ExecuteSqlCommand("TRUNCATE TABLE TransactionLogs");
                ctx.Database.ExecuteSqlCommand("TRUNCATE TABLE TransactionVMStacks");
            }
            catch
            {
                //ignore
            }
        }
    }

    [CollectionDefinition("SqlServerTestFixture")]
    public class SqlServerFixtureCollection : ICollectionFixture<SqlServerTestFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [Collection("SqlServerTestFixture")]
    public class SqlServerRepositoryTests : RepositoryLayerTestBase
    {
        public SqlServerRepositoryTests(SqlServerTestFixture fixture) : base(new BlockchainStoreRepositoryFactory(fixture.Factory))
        {
        }
    }
}
