using Nethereum.BlockchainProcessing.Storage.Entities;
using Nethereum.BlockchainStore.EF.SqlServer;

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
