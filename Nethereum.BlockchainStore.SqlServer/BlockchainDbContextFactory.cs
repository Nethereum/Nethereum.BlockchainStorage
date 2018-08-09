namespace Nethereum.BlockchainStore.SqlServer
{
    public class BlockchainDbContextFactory : IBlockchainDbContextFactory
    {
        private readonly string _connectionString;
        private readonly string _schema;

        public BlockchainDbContextFactory(string connectionString, string schema)
        {
            _connectionString = connectionString;
            _schema = schema;
        }

        public BlockchainDbContext CreateContext()
        {
            return new BlockchainDbContext(_connectionString, _schema);
        }
    }
}