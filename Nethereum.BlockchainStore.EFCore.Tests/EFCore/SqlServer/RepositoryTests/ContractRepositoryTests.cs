using Nethereum.BlockchainStore.Tests.EFCore.Base.RepositoryTests;
using Nethereum.BlockchainStore.Tests.EFCore.SqlServer.Common;

namespace Nethereum.BlockchainStore.Tests.EFCore.SqlServer.RepositoryTests
{
    public class ContractRepositoryTests: ContractRepositoryBaseTests
    {
        public ContractRepositoryTests():base(Utils.CreateSqlServerContextFactory())
        {
        }
    }
}
