using Nethereum.BlockchainStore.Tests.EFCore.Base.RepositoryTests;
using Utils = Nethereum.BlockchainStore.Tests.EFCore.SqlServer.Common.Utils;

namespace Nethereum.BlockchainStore.Tests.EFCore.SqlServer.RepositoryTests
{
    public class TransactionRepositoryTests: TransactionRepositoryBaseTests
    {
        public TransactionRepositoryTests():base(Utils.CreateSqlServerContextFactory())
        {
        }
    }
}
