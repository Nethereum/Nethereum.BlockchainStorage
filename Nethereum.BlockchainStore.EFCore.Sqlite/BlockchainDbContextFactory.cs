using System.Collections.Concurrent;
using Nethereum.BlockchainStore.EFCore;

namespace Nethereum.BlockchainStore.EFCore.Sqlite
{
    public class BlockchainDbContextFactory : IBlockchainDbContextFactory
    {
        private readonly string _connectionString;
        public readonly object _lock = new object();
        public bool _dbExists = false;

        public BlockchainDbContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BlockchainDbContextBase CreateContext()
        {
            var context = new BlockchainDbContext(_connectionString);
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