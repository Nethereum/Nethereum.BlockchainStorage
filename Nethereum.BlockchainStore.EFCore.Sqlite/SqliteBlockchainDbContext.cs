using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nethereum.BlockchainStore.EFCore.EntityBuilders;

namespace Nethereum.BlockchainStore.EFCore.Sqlite
{
    public class SqliteBlockchainDbContext: BlockchainDbContextBase
    {
        private readonly string _connectionString;

        public SqliteBlockchainDbContext()
        {
            _connectionString = GetConnectionString();
        }

        public SqliteBlockchainDbContext(string connectionString)
        {
            EntityBuilderExtensions.ColumnTypeForUnlimitedText = "TEXT";
            _connectionString = connectionString;
        }

        private string GetConnectionString()
        {
            var config = ConfigurationUtils.Build();
            var connectionStringName = $"BlockchainDbStorageDesignTime";
            var connectionString = config.GetConnectionString(connectionStringName);
            return connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            EntityBuilderExtensions.ColumnTypeForUnlimitedText = "TEXT";
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}
