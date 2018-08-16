using Nethereum.BlockchainStore.EF;

namespace Nethereum.BlockchainStore.EF.Sqlite
{
    public class BlockchainDbContextFactory : IBlockchainDbContextFactory
    {
        private readonly string _connectionName;

        public BlockchainDbContextFactory(string connectionName)
        {
            _connectionName = connectionName;
        }

        public BlockchainDbContextBase CreateContext()
        {
            return new BlockchainDbContext(_connectionName);
        }
    }
}