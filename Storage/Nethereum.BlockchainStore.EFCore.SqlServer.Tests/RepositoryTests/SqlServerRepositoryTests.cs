using System;
using Microsoft.EntityFrameworkCore;
using Nethereum.BlockchainStore.Test.Base.RepositoryTests;
using Xunit;

namespace Nethereum.BlockchainStore.EFCore.Tests.SqlServer.RepositoryTests
{
    public class SqlServerTestFixture : IDisposable
    {
        public SqlServerTestFixture()
        {
            Factory = new TestSqlServerBlockchainDbContextFactory();
            EmptyDatabase();
        }

        public TestSqlServerBlockchainDbContextFactory Factory { get; }

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
