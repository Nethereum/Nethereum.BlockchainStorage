using Nethereum.BlockchainStore.EFCore;

namespace Nethereum.BlockchainStore.EFCore.Sqlite
{
    public class BlockchainDbContextFactory : IBlockchainDbContextFactory
    {
        private readonly string _connectionString;

        public BlockchainDbContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BlockchainDbContextBase CreateContext()
        {
            return new BlockchainDbContext(_connectionString);
        }
    }
}