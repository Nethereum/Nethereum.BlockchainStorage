using Nethereum.BlockchainStore.SqlServer;

namespace Nethereum.BlockchainStore.Tests.EFCore.SqlServer.Common
{
    public static class Utils
    {
        public const string connectionString =
            "Data Source=localhost\\SQLEXPRESS01;Database=BlockchainStorage;Integrated Security=False;User ID=localhost1;Password=MeLLfMA1wBlJCzSGZhkO;Connect Timeout=60;";

        public static BlockchainDbContextFactory CreateSqlServerContextFactory(string schema = "localhost")
        {
            return  new BlockchainDbContextFactory(Utils.connectionString, schema);
        }
    }
}
