using Nethereum.BlockchainStore.EF;

namespace Nethereum.BlockchainStore.EF.Sqlite
{
    public class SqliteBlockchainDbContextFactory : IBlockchainDbContextFactory
    {
        private readonly string _connectionName;

        public SqliteBlockchainDbContextFactory(string connectionName)
        {
            _connectionName = connectionName;
        }

        public BlockchainDbContextBase CreateContext()
        {
            return new SqliteBlockchainDbContext(_connectionName);
        }
    }
}