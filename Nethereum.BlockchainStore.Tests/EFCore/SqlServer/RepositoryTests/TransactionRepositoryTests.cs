using System.Threading.Tasks;
using Nethereum.BlockchainStore.EFCore;
using Nethereum.BlockchainStore.EFCore.Repositories;
using Nethereum.BlockchainStore.EFCore.Tests.RepositoryTests;
using Nethereum.BlockchainStore.SqlServer;
using Xunit;
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
