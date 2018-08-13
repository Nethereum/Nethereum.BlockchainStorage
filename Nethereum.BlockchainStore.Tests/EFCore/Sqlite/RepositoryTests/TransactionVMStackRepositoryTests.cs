using Nethereum.BlockchainStore.Tests.EFCore.Base.RepositoryTests;

namespace Nethereum.BlockchainStore.Tests.EFCore.Sqlite.RepositoryTests
{
    public class TransactionVMStackRepositoryTests: TransactionVMStackRepositoryBaseTests
    {
        public TransactionVMStackRepositoryTests():base(Common.Utils.CreateSqliteContextFactory())
        {
        }
    }
}
