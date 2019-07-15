using Nethereum.BlockchainStore.Test.Base.RepositoryTests;
using Nethereum.BlockchainStore.EF;
using System;
using Xunit;
using Nethereum.BlockchainStore.EF.Hana;

namespace Nethereum.BlockchainStore.EF.Tests.Hana.RepositoryTests
{
    public class HanaTestFixture : IDisposable
    {
        public HanaTestFixture()
        {
            Factory = new TestHanaContextFactory();
            EmptyDatabase();
        }

        public TestHanaContextFactory Factory { get; }

        public void Dispose()
        {
            EmptyDatabase();
        }

        private void EmptyDatabase()
        {
            try
            {
                var ctx = Factory.CreateContext();
                var hctx = ctx as HanaBlockchainDbContext;                
                ctx.Database.ExecuteSqlCommand($"TRUNCATE TABLE \"{hctx.Schema}\".\"Blocks\"");
                ctx.Database.ExecuteSqlCommand($"TRUNCATE TABLE \"{hctx.Schema}\".\"Contracts\"");
                ctx.Database.ExecuteSqlCommand($"TRUNCATE TABLE \"{hctx.Schema}\".\"AddressTransactions\"");
                ctx.Database.ExecuteSqlCommand($"TRUNCATE TABLE \"{hctx.Schema}\".\"Transactions\"");
                ctx.Database.ExecuteSqlCommand($"TRUNCATE TABLE \"{hctx.Schema}\".\"TransactionLogs\"");
                ctx.Database.ExecuteSqlCommand($"TRUNCATE TABLE \"{hctx.Schema}\".\"TransactionVMStacks\"");
            }
            catch
            {          
                // ignore
            }
        }
    }

    [CollectionDefinition("HanaTestFixture")]
    public class HanaFixtureCollection : ICollectionFixture<HanaTestFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [Collection("HanaTestFixture")]
    public class HanaRepositoryTests : RepositoryLayerTestBase
    {
        public HanaRepositoryTests(HanaTestFixture fixture) : base(new BlockchainStoreRepositoryFactory(fixture.Factory))
        {
        }
    }
}
