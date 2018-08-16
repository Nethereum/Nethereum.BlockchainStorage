using Nethereum.BlockchainStore.EF.SqlServer;

namespace Nethereum.BlockchainStore.EF.Tests.SqlServer
{
    public class TestSqlServerContextFactory: BlockchainDbContextFactory
    {
        public TestSqlServerContextFactory():base(
            "BlockchainDbContext_sqlserver", 
            DbSchemaNames.localhost
            )
        {
            
        }
    }
}
