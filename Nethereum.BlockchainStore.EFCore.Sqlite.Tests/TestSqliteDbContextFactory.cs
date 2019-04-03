using Nethereum.BlockchainStore.EFCore.Sqlite;
using System.IO;

namespace Nethereum.BlockchainStore.EFCore.Tests.Sqlite
{
    public class TestSqliteDbContextFactory : SqliteBlockchainDbContextFactory
    {
        public static readonly object _lock = new object();
        public static bool _dbExists = false;

        public static readonly string ConnectionString =
            $"Data Source={Path.Combine(Path.GetTempPath(), "Blockchain_EFCore_UnitTest.db")}";

        public TestSqliteDbContextFactory() : base(ConnectionString)
        {
            if (!_dbExists)
            {
                lock (_lock)
                {
                    if (!_dbExists)
                    {
                        new SqliteBlockchainDbContext(ConnectionString).Database.EnsureCreated();
                        _dbExists = true;
                    }
                }
            }
        }
    }
}
