using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nethereum.BlockchainStore.EF.Tests.Base.RepositoryTests;

namespace Nethereum.BlockchainStore.EF.Tests.Sqlite.RepositoryTests
{
    [TestClass]
    public class BlockRepositoryTests: BlockRepositoryBaseTests
    {
        public BlockRepositoryTests():base(new TestSqliteContextFactory())
        {
        }
    }
}
