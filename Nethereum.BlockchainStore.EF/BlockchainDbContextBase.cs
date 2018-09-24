using System;
using System.Data.Entity;
using Nethereum.BlockchainStore.Entities;
using BlockEntityBuilder = Nethereum.BlockchainStore.EF.EntityBuilders.BlockEntityBuilder;
using ContractEntityBuilder = Nethereum.BlockchainStore.EF.EntityBuilders.ContractEntityBuilder;
using TransactionEntityBuilder = Nethereum.BlockchainStore.EF.EntityBuilders.TransactionEntityBuilder;
using TransactionLogEntityBuilder = Nethereum.BlockchainStore.EF.EntityBuilders.TransactionLogEntityBuilder;
using TransactionLogVmStackEntityBuilder = Nethereum.BlockchainStore.EF.EntityBuilders.TransactionLogVmStackEntityBuilder;

namespace Nethereum.BlockchainStore.EF
{
    public abstract class BlockchainDbContextBase: DbContext
    {
        protected BlockchainDbContextBase(){}
        protected BlockchainDbContextBase(string connectionName) : base(connectionName){}

        public DbSet<Block> Blocks { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<TransactionLog> TransactionLogs { get; set; }
        public DbSet<TransactionVmStack> TransactionVmStacks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new BlockEntityBuilder());
            modelBuilder.Configurations.Add(new ContractEntityBuilder());
            modelBuilder.Configurations.Add(new TransactionEntityBuilder());
            modelBuilder.Configurations.Add(new TransactionLogEntityBuilder());
            modelBuilder.Configurations.Add(new TransactionLogVmStackEntityBuilder());
            base.OnModelCreating(modelBuilder);
        }
    }
}
