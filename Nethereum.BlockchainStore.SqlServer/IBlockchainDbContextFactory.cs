namespace Nethereum.BlockchainStore.SqlServer
{
    public interface IBlockchainDbContextFactory
    {
        BlockchainDbContext CreateContext();
    }
}