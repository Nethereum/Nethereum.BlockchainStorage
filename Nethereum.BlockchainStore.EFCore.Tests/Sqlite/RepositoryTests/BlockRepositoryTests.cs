using Nethereum.BlockchainStore.EFCore.Tests.Base.RepositoryTests;

namespace Nethereum.BlockchainStore.EFCore.Tests.Sqlite.RepositoryTests
{
    public class BlockRepositoryTests: BlockRepositoryBaseTests
    {
        public BlockRepositoryTests():base(new TestSqliteDbContextFactory())
        {
        }
    }
}
