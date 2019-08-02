using System;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Nethereum.Microsoft.Configuration.Utils;

namespace Nethereum.BlockchainStore.EFCore.SqlServer
{
    /*
        The classes below support EF migrations
        For each blockchain test net we have a different SQL schema
        Each schema has the same tables
        It means the same db can store data for different block chains
        It relies on the appsettings.json file
        It should contain a SQL server connection string called BlockchainDbStorageDesignTime
        This must have the necessary permissions to read and create objects
     */

    public class BlockchainDbContextDesignTimeFactory_dbo: BlockchainDbContextDesignTimeFactory<SqlServerBlockchainDbContext_dbo>{}
    public class BlockchainDbContextDesignTimeFactory_localhost: BlockchainDbContextDesignTimeFactory<SqlServerBlockchainDbContext_localhost>{}
    public class BlockchainDbContextDesignTimeFactory_rinkeby: BlockchainDbContextDesignTimeFactory<SqlServerBlockchainDbContext_rinkeby>{}
    public class BlockchainDbContextDesignTimeFactory_kovan: BlockchainDbContextDesignTimeFactory<SqlServerBlockchainDbContext_kovan>{}
    public class BlockchainDbContextDesignTimeFactory_ropsten: BlockchainDbContextDesignTimeFactory<SqlServerBlockchainDbContext_ropsten>{} 
    public class BlockchainDbContextDesignTimeFactory_main: BlockchainDbContextDesignTimeFactory<SqlServerBlockchainDbContext_main>{}
   
    public abstract class BlockchainDbContextDesignTimeFactory<T> : IDesignTimeDbContextFactory<T>
        where T : SqlServerBlockchainDbContext
    {
        public T CreateDbContext(string[] args)
        {
            return (T)Activator.CreateInstance(typeof(T), new object[]{GetConnectionString()});
        }

        protected virtual string GetConnectionString()
        {
            var config = ConfigurationUtils.Build();
            var connectionStringName = $"BlockchainDbStorageDesignTime";
            var connectionString = config.GetConnectionString(connectionStringName);
            return connectionString;
        }
    }
   
}
