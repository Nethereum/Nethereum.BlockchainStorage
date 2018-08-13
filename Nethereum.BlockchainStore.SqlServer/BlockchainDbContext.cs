﻿using Microsoft.EntityFrameworkCore;
using Nethereum.Blockchain.EFCore;
using Nethereum.BlockchainStore.EFCore.EntityBuilders;

namespace Nethereum.BlockchainStore.SqlServer
{
    public class BlockchainDbContext: BlockchainDbContextBase
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
