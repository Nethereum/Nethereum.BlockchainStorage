using System.Threading.Tasks;
using Nethereum.BlockchainStore.EFCore;
using Nethereum.BlockchainStore.EFCore.Repositories;
using Nethereum.BlockchainStore.Tests.EFCore.Base.RepositoryTests;
using Xunit;

namespace Nethereum.BlockchainStore.Tests.EFCore.Sqlite.RepositoryTests
{
    public class TransactionRepositoryTests: TransactionRepositoryBaseTests
    {
        public TransactionRepositoryTests():base(Common.Utils.CreateSqliteContextFactory())
        {
        }
    }
}
