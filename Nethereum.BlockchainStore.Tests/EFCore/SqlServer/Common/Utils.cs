using Nethereum.BlockchainStore.SqlServer;

namespace Nethereum.BlockchainStore.Tests.EFCore.SqlServer.Common
{
    public static class Utils
    {
        public const string connectionString =
            "Data Source=davewhiffin.database.windows.net;Database=BlockchainStorage;Integrated Security=False;User ID=localhost1;Password=MeLLfMA1wBlJCzSGZhkO;Connect Timeout=60;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public static BlockchainDbContextFactory CreateSqlServerContextFactory(string schema = "localhost")
        {
            return  new BlockchainDbContextFactory(Utils.connectionString, schema);
        }
    }
}
