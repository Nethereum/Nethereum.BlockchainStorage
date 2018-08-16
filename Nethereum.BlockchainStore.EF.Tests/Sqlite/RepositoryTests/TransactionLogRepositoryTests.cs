using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nethereum.BlockchainStore.EF.Tests.Base.RepositoryTests;

namespace Nethereum.BlockchainStore.EF.Tests.Sqlite.RepositoryTests
{
    [TestClass]
    public class TransactionLogRepositoryTests: TransactionLogRepositoryBaseTests
    {
        public TransactionLogRepositoryTests():base(new TestSqliteContextFactory())
        {
        }
    }
}
