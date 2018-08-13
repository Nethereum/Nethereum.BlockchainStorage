using System;
using Microsoft.EntityFrameworkCore;
using Nethereum.Blockchain.EFCore;
using Nethereum.BlockchainStore.EFCore.EntityBuilders;

namespace Nethereum.Blockchain.Sqlite
{
    public class BlockchainDbContext: BlockchainDbContextBase
    {
        private readonly string _connectionString;

        public BlockchainDbContext()
        {
            _connectionString =
                "Data Source=C:/dev/repos/Nethereum.BlockchainStorage/Nethereum.BlockchainStore.Sqlite/Blockchain.db";
        }

        public BlockchainDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            EntityBuilderExtensions.ColumnTypeForUnlimitedText = "TEXT";
            optionsBuilder.UseSqlite(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BlockEntityBuilder());
            modelBuilder.ApplyConfiguration(new ContractEntityBuilder());
            modelBuilder.ApplyConfiguration(new TransactionEntityBuilder());
            modelBuilder.ApplyConfiguration(new TransactionLogEntityBuilder());
            modelBuilder.ApplyConfiguration(new TransactionLogVmStackEntityBuilder());
            base.OnModelCreating(modelBuilder);
        }
    }
}
