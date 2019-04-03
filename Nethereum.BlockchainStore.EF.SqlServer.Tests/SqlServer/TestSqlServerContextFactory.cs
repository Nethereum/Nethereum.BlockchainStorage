using Nethereum.BlockchainStore.EF.SqlServer;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.EF.Tests.SqlServer
{
    public class TestSqlServerContextFactory: SqlServerBlockchainDbContextFactory
    {
        public TestSqlServerContextFactory():base(
            "BlockchainDbContext_sqlserver", 
            DbSchemaNames.dbo
            )
        {
            
        }
    }
}
