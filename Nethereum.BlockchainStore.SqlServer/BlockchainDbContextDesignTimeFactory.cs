using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace Nethereum.BlockchainStore.SqlServer
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

    public class BlockchainDbContextDesignTimeFactory_localhost: BlockchainDbContextDesignTimeFactory<BlockchainDbContext_localhost>{}
    public class BlockchainDbContextDesignTimeFactory_rinkeby: BlockchainDbContextDesignTimeFactory<BlockchainDbContext_rinkeby>{}
    public class BlockchainDbContextDesignTimeFactory_kovan: BlockchainDbContextDesignTimeFactory<BlockchainDbContext_kovan>{}
    public class BlockchainDbContextDesignTimeFactory_ropsten: BlockchainDbContextDesignTimeFactory<BlockchainDbContext_ropsten>{} 
    public class BlockchainDbContextDesignTimeFactory_main: BlockchainDbContextDesignTimeFactory<BlockchainDbContext_main>{}
   
    public abstract class BlockchainDbContextDesignTimeFactory<T> : IDesignTimeDbContextFactory<T>
        where T : BlockchainDbContext
    {
        public T CreateDbContext(string[] args)
        {
            return (T)Activator.CreateInstance(typeof(T), new object[]{GetConnectionString()});
        }

        protected virtual string GetConnectionString()
        {
            var config = ConfigurationUtils.Build(this.GetType());
            var connectionStringName = $"BlockchainDbStorageDesignTime";
            var connectionString = config.GetConnectionString(connectionStringName);
            return connectionString;
        }
    }
   
}
