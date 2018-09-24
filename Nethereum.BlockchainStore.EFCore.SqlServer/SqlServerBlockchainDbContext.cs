using Microsoft.EntityFrameworkCore;
using Nethereum.BlockchainStore.EFCore.EntityBuilders;
using System;
using System.Reflection;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.EFCore.SqlServer
{
    [DbSchema(DbSchemaNames.dbo)]
    public class SqlServerBlockchainDbContext_dbo: SqlServerBlockchainDbContext
    {
        public SqlServerBlockchainDbContext_dbo(string connectionString) : base(connectionString){}
    }

    [DbSchema(DbSchemaNames.localhost)]
    public class SqlServerBlockchainDbContext_localhost : SqlServerBlockchainDbContext
    {
        public SqlServerBlockchainDbContext_localhost(string connectionString) : base(connectionString){}
    }

    [DbSchema(DbSchemaNames.rinkeby)]
    public class SqlServerBlockchainDbContext_rinkeby : SqlServerBlockchainDbContext
    {
        public SqlServerBlockchainDbContext_rinkeby(string connectionString) : base(connectionString){}
    }

    [DbSchema(DbSchemaNames.kovan)]
    public class SqlServerBlockchainDbContext_kovan : SqlServerBlockchainDbContext
    {
        public SqlServerBlockchainDbContext_kovan(string connectionString) : base(connectionString){}
    }

    [DbSchema(DbSchemaNames.main)]
    public class SqlServerBlockchainDbContext_main : SqlServerBlockchainDbContext
    {
        public SqlServerBlockchainDbContext_main(string connectionString) : base(connectionString){}
    }

    [DbSchema(DbSchemaNames.ropsten)]
    public class SqlServerBlockchainDbContext_ropsten : SqlServerBlockchainDbContext
    {
        public SqlServerBlockchainDbContext_ropsten(string connectionString) : base(connectionString){}
    }

    public abstract class SqlServerBlockchainDbContext: BlockchainDbContextBase
    {
        private readonly string _connectionString;

        public string Schema { get; }

        protected SqlServerBlockchainDbContext(string connectionString)
        {
            _connectionString = connectionString;

            var dbSchemaAttribute = (DbSchemaAttribute)this.GetType().GetCustomAttribute(typeof(DbSchemaAttribute));
            if(dbSchemaAttribute == null)
                throw new Exception("Type requires a DBSchema custom attribute");

            Schema = dbSchemaAttribute.DbSchemaName.ToString();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            EntityBuilderExtensions.ColumnTypeForUnlimitedText = "nvarchar(max)";
            optionsBuilder.UseSqlServer(_connectionString, (a) =>
            {
                
            });
        }
    }
}
