using System;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

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

        string RecurseUntilFound(string directory, string fileToFind)
        {
            if (Directory.GetFiles(directory, fileToFind).Length > 0)
            {
                return directory;
            }

            var parent = Directory.GetParent(directory);
            if (parent == null)
                return null;

            return RecurseUntilFound(parent.FullName, fileToFind);
        }

        string FindAppSettingsDirectory()
        {
            var assemblyFilePath = typeof(BlockchainDbContextDesignTimeFactory).Assembly.Location;
            var startingDirectory = Path.GetDirectoryName(assemblyFilePath);
            return RecurseUntilFound(startingDirectory, "appsettings.json");
        }
        
        public BlockchainDbContext CreateDbContext(string[] args)
        {
            var path = FindAppSettingsDirectory();

            //TODO: !!! 
            path = @"C:\dev\repos\Nethereum.BlockchainStorage\Nethereum.BlockchainStore.SqlServer";

            if (path == null)
                throw new Exception("appsettings.json file could not be found");

            var config = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            //var connectionStringName = GetEnvironmentVariable("connectionName", "Unrecognised connection name");
            var connectionString = config.GetConnectionString("BlockchainDbStorageDesignTime_localhost");
            var schema = config.GetValue<string>("BlockchainDbStorageDesignTimeSchema");

            return new BlockchainDbContext(connectionString, schema);
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
    }     
}
