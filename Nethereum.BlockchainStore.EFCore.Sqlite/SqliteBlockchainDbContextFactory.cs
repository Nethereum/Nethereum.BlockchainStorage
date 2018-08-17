namespace Nethereum.BlockchainStore.EFCore.Sqlite
{
    public class SqliteBlockchainDbContextFactory : IBlockchainDbContextFactory
    {
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
                    if (_dbExists)
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