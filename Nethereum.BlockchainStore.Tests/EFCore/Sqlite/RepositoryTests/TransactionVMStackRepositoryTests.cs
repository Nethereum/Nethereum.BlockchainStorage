using Nethereum.BlockchainStore.EFCore.Tests.RepositoryTests;

namespace Nethereum.BlockchainStore.Tests.EFCore.Sqlite.RepositoryTests
{
    public class TransactionVMStackRepositoryTests: TransactionVMStackRepositoryBaseTests
    {
        public TransactionVMStackRepositoryTests():base(Common.Utils.CreateSqliteContextFactory())
        {
        }
    }
}
