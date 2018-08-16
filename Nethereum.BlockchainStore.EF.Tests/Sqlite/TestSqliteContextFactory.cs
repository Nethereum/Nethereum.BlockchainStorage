using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.EF.Sqlite;

namespace Nethereum.BlockchainStore.EF.Tests.Sqlite
{
    public class TestSqliteContextFactory : BlockchainDbContextFactory
    {
        public TestSqliteContextFactory() : base("BlockchainDbContext_sqlite")
        {
        }
    }
}
