using Microsoft.EntityFrameworkCore;
using Nethereum.BlockchainStore.SqlServer.Entities;
using Nethereum.BlockchainStore.SqlServer.Entities.Builders;

namespace Nethereum.BlockchainStore.SqlServer
{
    public class BlockchainDbContext: DbContext
    {
        private readonly string _connectionString;
        private readonly string _schema;

        public BlockchainDbContext(string connectionString, string schema)
        {
            _connectionString = connectionString;
            _schema = schema;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(_schema);
            modelBuilder.ApplyConfiguration(new BlockModelBuilder());
            modelBuilder.ApplyConfiguration(new ContractModelBuilder());
            modelBuilder.ApplyConfiguration(new TransactionModelBuilder());
            modelBuilder.ApplyConfiguration(new TransactionLogModelBuilder());
            modelBuilder.ApplyConfiguration(new TransactionLogVmStackModelBuilder());
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString, (a) =>
            {
                
            });
        }

        public DbSet<Block> Blocks { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<TransactionLog> TransactionLogs { get; set; }
        public DbSet<TransactionVmStack> TransactionVmStacks { get; set; }
    }
}
