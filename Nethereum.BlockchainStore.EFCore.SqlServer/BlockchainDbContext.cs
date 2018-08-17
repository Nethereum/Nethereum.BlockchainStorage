using Microsoft.EntityFrameworkCore;
using Nethereum.BlockchainStore.EFCore.EntityBuilders;
using System;
using System.Reflection;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.EFCore.SqlServer
{
    [DbSchema(DbSchemaNames.dbo)]
    public class BlockchainDbContext_dbo: BlockchainDbContext
    {
        public BlockchainDbContext_dbo(string connectionString) : base(connectionString){}
    }

    [DbSchema(DbSchemaNames.localhost)]
    public class BlockchainDbContext_localhost : BlockchainDbContext
    {
        public BlockchainDbContext_localhost(string connectionString) : base(connectionString){}
    }

    [DbSchema(DbSchemaNames.rinkeby)]
    public class BlockchainDbContext_rinkeby : BlockchainDbContext
    {
        public BlockchainDbContext_rinkeby(string connectionString) : base(connectionString){}
    }

    [DbSchema(DbSchemaNames.kovan)]
    public class BlockchainDbContext_kovan : BlockchainDbContext
    {
        public BlockchainDbContext_kovan(string connectionString) : base(connectionString){}
    }

    [DbSchema(DbSchemaNames.main)]
    public class BlockchainDbContext_main : BlockchainDbContext
    {
        public BlockchainDbContext_main(string connectionString) : base(connectionString){}
    }

    [DbSchema(DbSchemaNames.ropsten)]
    public class BlockchainDbContext_ropsten : BlockchainDbContext
    {
        public BlockchainDbContext_ropsten(string connectionString) : base(connectionString){}
    }

    public abstract class BlockchainDbContext: BlockchainDbContextBase
    {
        private readonly string _connectionString;

        public string Schema { get; }

        protected BlockchainDbContext(string connectionString)
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
