using Nethereum.Blockchain.EFCore;
using Nethereum.Blockchain.Sqlite;
using Nethereum.BlockchainStore.EFCore;

namespace Nethereum.BlockchainStore.Sqlite
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
            return new BlockchainDbContext();
        }
    }
}