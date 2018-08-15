using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nethereum.Blockchain.EFCore;
using Nethereum.BlockchainStore.EFCore.EntityBuilders;

namespace Nethereum.BlockchainStore.SqlServer
{
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
            modelBuilder.ApplyConfiguration(new BlockEntityBuilder());
            modelBuilder.ApplyConfiguration(new ContractEntityBuilder());
            modelBuilder.ApplyConfiguration(new TransactionEntityBuilder());
            modelBuilder.ApplyConfiguration(new TransactionLogEntityBuilder());
            modelBuilder.ApplyConfiguration(new TransactionLogVmStackEntityBuilder());
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
