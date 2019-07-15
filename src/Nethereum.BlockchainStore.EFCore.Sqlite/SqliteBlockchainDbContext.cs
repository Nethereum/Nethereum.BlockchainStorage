using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Configuration.Utils;

namespace Nethereum.BlockchainStore.EFCore.Sqlite
{
    public class SqliteBlockchainDbContext: BlockchainDbContextBase
    {
        private readonly string _connectionString;

        public SqliteBlockchainDbContext():this(GetConnectionString())
        {
        }

        public SqliteBlockchainDbContext(string connectionString)
        {
            ColumnTypeForUnlimitedText = "TEXT";
            _connectionString = connectionString;
        }

        private static string GetConnectionString()
        {
            var config = ConfigurationUtils.Build();
            var connectionStringName = $"BlockchainDbStorageDesignTime";
            var connectionString = config.GetConnectionString(connectionStringName);
            return connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}
