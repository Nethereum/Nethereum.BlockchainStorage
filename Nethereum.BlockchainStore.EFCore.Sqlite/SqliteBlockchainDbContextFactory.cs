using Microsoft.Extensions.Configuration;

namespace Nethereum.BlockchainStore.EFCore.Sqlite
{
    public class SqliteBlockchainDbContextFactory : IBlockchainDbContextFactory
    {
        public static SqliteBlockchainDbContextFactory Create(IConfigurationRoot config)
        {
            var connectionString = config.GetBlockchainStorageConnectionString();
            return new SqliteBlockchainDbContextFactory(connectionString);
        }

        private readonly string _connectionString;
        private readonly object _lock = new object();
        private bool _dbExists = false;

        public SqliteBlockchainDbContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BlockchainDbContextBase CreateContext()
        {
            var context = new SqliteBlockchainDbContext(_connectionString);
            if (!_dbExists)
            {
                lock (_lock)
                {
                    if (!_dbExists)
                    {
                        context.Database.EnsureCreated();
                        _dbExists = true;
                    }
                }
            }
            return context;
        }
    }
}