using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

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
        /// <summary>
        /// Creates a DB context for EF migrations
        /// WARNING! At present an appsettings.json file must be in a child folder of the executable
        /// It expects the desired schema to be set in an environmental variable called BlockchainDbStorageDesignTimeSchema
        /// Acceptable values are: localhost, kovan, ropsten, rinkeby, main
        /// localhost is default
        /// It will then lookup the related connection string from an appsettings file (e.g. "BlockchainDbStorageDesignTime_localhost")
        /// This config file dictates the database connection and schema used to create migrations
        /// </summary>
        /// <param name="args">args is not passed by the dot net framework - ignore this</param>
        /// <returns></returns>
        public BlockchainDbContext CreateDbContext(string[] args)
        {
            var config = ConfigurationUtils.Build(this.GetType());

            var schemaName = GetSchemaFromEnvironmentVariables();
            var connectionStringName = $"BlockchainDbStorageDesignTime_{schemaName}";
            var connectionString = config.GetConnectionString(connectionStringName);

            if(string.IsNullOrEmpty(connectionString))
                throw new Exception($"Null or empty connection string for connection string name: {connectionStringName}.  Check the appsettings.json file.");

            return new BlockchainDbContext(connectionString, schemaName);
        }

        private string GetSchemaFromEnvironmentVariables()
        {
            return GetEnvironmentVariable(name: "BlockchainDbStorageDesignTimeSchema", defaultValue: "localhost");
        }

        private string GetEnvironmentVariable(string name, string defaultValue)
        {
            var connectionStringName = Environment.GetEnvironmentVariable(name);

            if (string.IsNullOrWhiteSpace(connectionStringName))
            {
                throw new Exception(defaultValue);
            }

            return connectionStringName;
        }
    }     
}
