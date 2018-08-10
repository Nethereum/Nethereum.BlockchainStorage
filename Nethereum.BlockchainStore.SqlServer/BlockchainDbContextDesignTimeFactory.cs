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
        public const string connectionString =
            "Data Source=davewhiffin.database.windows.net;Database=BlockchainStorage;Integrated Security=False;User ID=davewhiffin;Password=G4@BJMQvCZ7e|@5b;Connect Timeout=60;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public BlockchainDbContext CreateDbContext(string[] args)
        {
            return new BlockchainDbContext(connectionString, "localhost");
        }
    }

    /*
    public EspaceBiereContext CreateDbContext(string[] args)
    {
        var appSettingsFinder = new DefaultAppSettingsFinder();
        var path = appSettingsFinder.FindPath<DbContextFactory>("EspaceBiere.sln", "EspaceBiere.Web");
        var config = new ConfigBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
            20
        var connectionStringName = GetEnvironmentVariable("ConnName", ConnNameError);
        var connectionString = config.GetConnectionString(connectionStringName);
        var builder = new DbContextOptionsBuilder<EspaceBiereContext>();
        builder.UseSqlServer(connectionString);

        return new EspaceBiereContext(builder.Options);
    }

    private string GetEnvironmentVariable(string name, string errorMessage)
    {
        var connectionStringName = Environment.GetEnvironmentVariable(name);

        if (string.IsNullOrWhiteSpace(connectionStringName))
        {
            throw new Exception(errorMessage);
        }

        return connectionStringName;
    }
     */
}
