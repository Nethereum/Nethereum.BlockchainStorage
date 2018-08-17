using Nethereum.BlockchainStore.EFCore.Sqlite;

namespace Nethereum.BlockchainStore.EFCore.Tests.Sqlite
{
    public class TestSqliteDbContextFactory : BlockchainDbContextFactory
    {
        public const string ConnectionString =
            "Data Source=C:/dev/repos/Nethereum.BlockchainStorage/Nethereum.BlockchainStore.EFCore.Sqlite/Blockchain.db";

        public TestSqliteDbContextFactory() : base(ConnectionString)
        {
        }
    }
}
