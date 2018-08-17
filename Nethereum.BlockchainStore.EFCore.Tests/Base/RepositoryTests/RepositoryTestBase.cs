namespace Nethereum.BlockchainStore.EFCore.Tests.Base.RepositoryTests
{
    public class RepositoryTestBase
    {
        protected readonly IBlockchainDbContextFactory contextFactory;

        public RepositoryTestBase(IBlockchainDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }
    }
}
