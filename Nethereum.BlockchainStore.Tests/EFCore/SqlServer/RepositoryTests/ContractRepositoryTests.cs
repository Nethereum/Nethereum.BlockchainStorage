using Nethereum.BlockchainStore.EFCore.Tests.RepositoryTests;
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
