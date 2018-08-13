using Nethereum.BlockchainStore.Sqlite;

namespace Nethereum.BlockchainStore.Tests.EFCore.Sqlite.Common
{
    public static class Utils
    {
        public const string connectionString =
            "Data Source=C:/dev/repos/Nethereum.BlockchainStorage/Nethereum.BlockchainStore.Sqlite/Blockchain.db";

        public static BlockchainDbContextFactory CreateSqliteContextFactory()
        {
            return  new BlockchainDbContextFactory(Utils.connectionString);
        }
    }
}
