using Nethereum.BlockchainStore.EFCore.Tests.Base.RepositoryTests;

namespace Nethereum.BlockchainStore.EFCore.Tests.SqlServer.RepositoryTests
{
    public class ContractRepositoryTests: ContractRepositoryBaseTests
    {
        public ContractRepositoryTests():base(new TestSqlServerBlockchainDbContextFactory())
        {
        }
    }
}
