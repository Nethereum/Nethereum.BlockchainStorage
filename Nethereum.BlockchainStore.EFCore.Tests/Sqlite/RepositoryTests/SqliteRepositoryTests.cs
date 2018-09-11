using System;
using System.Collections.Generic;
using System.Text;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.Test.Base.RepositoryTests;

namespace Nethereum.BlockchainStore.EFCore.Tests.Sqlite.RepositoryTests
{
    public class SqliteRepositoryTests : RepositoryLayerTestBase
    {
        public SqliteRepositoryTests() : base(new BlockchainStoreRepositoryFactory(new TestSqliteDbContextFactory()))
        {
        }
    }
}
