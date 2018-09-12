using Nethereum.BlockchainStore.EFCore.SqlServer;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.EFCore.Tests.SqlServer
{
    public class TestSqlServerBlockchainDbContextFactory : SqlServerCoreBlockchainDbContextFactory
    {
        public const string ConnectionString =
            "Data Source=localhost\\SQLEXPRESS01;Database=BlockchainStorage;Integrated Security=False;User ID=blockchaindbo1;Password=bALLfMA1wBlJCzSGZhkO;Connect Timeout=60;";

        public TestSqlServerBlockchainDbContextFactory() : base(ConnectionString, DbSchemaNames.dbo)
        {
        }
    }
}
