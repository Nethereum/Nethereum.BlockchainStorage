using Nethereum.BlockchainStore.EFCore;

namespace Nethereum.BlockchainStore.Tests.EFCore.Base.RepositoryTests
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
