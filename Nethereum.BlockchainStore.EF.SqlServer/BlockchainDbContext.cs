using Nethereum.BlockchainStore.EF.EntityBuilders;
using System;
using System.Data.Entity;
using System.Reflection;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.EF.SqlServer
{
    [DbSchema(DbSchemaNames.dbo)]
    public class BlockchainDbContext_dbo : BlockchainDbContext
    {
        public BlockchainDbContext_dbo(string connectionName) : base(connectionName){}
    }
    
    [DbSchema(DbSchemaNames.localhost)]
    public class BlockchainDbContext_localhost : BlockchainDbContext
    {
        public BlockchainDbContext_localhost(string connectionName) : base(connectionName){}
    }

    [DbSchema(DbSchemaNames.rinkeby)]
    public class BlockchainDbContext_rinkeby : BlockchainDbContext
    {
        public BlockchainDbContext_rinkeby(string connectionName) : base(connectionName){}
    }

    [DbSchema(DbSchemaNames.kovan)]
    public class BlockchainDbContext_kovan : BlockchainDbContext
    {
        public BlockchainDbContext_kovan(string connectionName) : base(connectionName){}
    }

    [DbSchema(DbSchemaNames.main)]
    public class BlockchainDbContext_main : BlockchainDbContext
    {
        public BlockchainDbContext_main(string connectionName) : base(connectionName){}
    }

    [DbSchema(DbSchemaNames.ropsten)]
    public class BlockchainDbContext_ropsten : BlockchainDbContext
    {
        public BlockchainDbContext_ropsten(string connectionName) : base(connectionName){}
    }

    public abstract class BlockchainDbContext: BlockchainDbContextBase
    {
        protected string Schema;

        protected BlockchainDbContext(string connectionName) : base(connectionName)
        {
            var dbSchemaAttribute = (DbSchemaAttribute)this.GetType().GetCustomAttribute(typeof(DbSchemaAttribute));
            if(dbSchemaAttribute == null)
                throw new Exception("Type requires a DBSchema custom attribute");

            Schema = dbSchemaAttribute.DbSchemaName.ToString();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);
            EntityBuilderExtensions.ColumnTypeForUnlimitedText = "nvarchar(max)";
            Database.SetInitializer<BlockchainDbContext>(null);
            //Database.SetInitializer(new CreateDatabaseIfNotExists<BlockchainDbContext>());
            base.OnModelCreating(modelBuilder);
        }
    }
}
