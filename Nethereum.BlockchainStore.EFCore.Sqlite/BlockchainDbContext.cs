﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nethereum.BlockchainStore.EFCore;
using Nethereum.BlockchainStore.EFCore.EntityBuilders;

namespace Nethereum.BlockchainStore.EFCore.Sqlite
{
    public class BlockchainDbContext: BlockchainDbContextBase
    {
        private readonly string _connectionString;

        public BlockchainDbContext()
        {
            _connectionString = GetConnectionString();
        }

        public BlockchainDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        private string GetConnectionString()
        {
            var config = ConfigurationUtils.Build(this.GetType());
            var connectionStringName = $"BlockchainDbStorageDesignTime";
            var connectionString = config.GetConnectionString(connectionStringName);
            return connectionString;
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
