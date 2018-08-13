using Nethereum.BlockchainStore.EFCore.Tests.RepositoryTests;
using Nethereum.BlockchainStore.Tests.EFCore.Sqlite.Common;

namespace Nethereum.BlockchainStore.Tests.EFCore.Sqlite.RepositoryTests
{
    public class TransactionLogRepositoryTests: TransactionLogRepositoryBaseTests
    {
        public TransactionLogRepositoryTests():base(Utils.CreateSqliteContextFactory())
        {
        }
    }
}
