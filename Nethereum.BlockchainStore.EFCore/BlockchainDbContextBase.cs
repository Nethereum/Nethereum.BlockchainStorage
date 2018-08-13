using System;
using Microsoft.EntityFrameworkCore;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.EFCore.EntityBuilders;

namespace Nethereum.Blockchain.EFCore
{
    public abstract class BlockchainDbContextBase: DbContext
    {
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<TransactionLog> TransactionLogs { get; set; }
        public DbSet<TransactionVmStack> TransactionVmStacks { get; set; }

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
