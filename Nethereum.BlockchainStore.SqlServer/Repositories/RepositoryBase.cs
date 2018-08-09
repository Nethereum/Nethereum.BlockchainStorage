namespace Nethereum.BlockchainStore.SqlServer.Repositories
{
    public abstract class RepositoryBase
    {
        protected readonly IBlockchainDbContextFactory _contextFactory;

        protected RepositoryBase(IBlockchainDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }
    }
}
