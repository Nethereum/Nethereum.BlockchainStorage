using Microsoft.EntityFrameworkCore.Design;

namespace Nethereum.BlockchainStore.SqlServer
{
    /*
     * MIGRATIONS
     *
     * dotnet ef migrations add Migration1
     * dotnet ef database update
     *
     * TO REVERT A MIGRATION
     * dotnet ef migrations remove -f
     */

    public class BlockchainDbContextDesignTimeFactory : IDesignTimeDbContextFactory<BlockchainDbContext>
    {
        const string connectionString =
            "Data Source=davewhiffin.database.windows.net;Database=BlockchainStorage;Integrated Security=False;User ID=localhost1;Password=MeLLfMA1wBlJCzSGZhkO;Connect Timeout=60;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public BlockchainDbContext CreateDbContext(string[] args)
        {
            return new BlockchainDbContext(connectionString, "localhost");
        }
    }
}
