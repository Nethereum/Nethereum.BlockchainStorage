using Nethereum.BlockchainProcessing.Storage.Entities;
using Nethereum.BlockchainStore.EF.EntityBuilders;
using System;
using System.Data.Entity;
using System.Reflection;

namespace Nethereum.BlockchainStore.EF.SqlServer
{
    [DbSchema(DbSchemaNames.dbo)]
    public class SqlServerSqlServerBlockchainDbContextDbo : SqlServerBlockchainDbContext
    {
        public SqlServerSqlServerBlockchainDbContextDbo(string connectionName) : base(connectionName){}
    }
    
    [DbSchema(DbSchemaNames.localhost)]
    public class SqlServerSqlServerBlockchainDbContextLocalhost : SqlServerBlockchainDbContext
    {
        public SqlServerSqlServerBlockchainDbContextLocalhost(string connectionName) : base(connectionName){}
    }

    [DbSchema(DbSchemaNames.rinkeby)]
    public class SqlServerSqlServerBlockchainDbContextRinkeby : SqlServerBlockchainDbContext
    {
        public SqlServerSqlServerBlockchainDbContextRinkeby(string connectionName) : base(connectionName){}
    }

    [DbSchema(DbSchemaNames.kovan)]
    public class SqlServerSqlServerBlockchainDbContextKovan : SqlServerBlockchainDbContext
    {
        public SqlServerSqlServerBlockchainDbContextKovan(string connectionName) : base(connectionName){}
    }

    [DbSchema(DbSchemaNames.main)]
    public class SqlServerSqlServerBlockchainDbContextMain : SqlServerBlockchainDbContext
    {
        public SqlServerSqlServerBlockchainDbContextMain(string connectionName) : base(connectionName){}
    }

    [DbSchema(DbSchemaNames.ropsten)]
    public class SqlServerSqlServerBlockchainDbContextRopsten : SqlServerBlockchainDbContext
    {
        public SqlServerSqlServerBlockchainDbContextRopsten(string connectionName) : base(connectionName){}
    }

    public abstract class SqlServerBlockchainDbContext: BlockchainDbContextBase
    {
        protected string Schema;

        protected SqlServerBlockchainDbContext(string connectionName) : base(connectionName)
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
            Database.SetInitializer<SqlServerBlockchainDbContext>(null);
            //Database.SetInitializer(new CreateDatabaseIfNotExists<BlockchainDbContext>());
            base.OnModelCreating(modelBuilder);
        }
    }
}
