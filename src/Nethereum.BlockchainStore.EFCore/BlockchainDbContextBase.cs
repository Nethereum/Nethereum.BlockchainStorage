using Microsoft.EntityFrameworkCore;
using Nethereum.BlockchainProcessing.Storage.Entities;
using Nethereum.BlockchainStore.EFCore.EntityBuilders;

namespace Nethereum.BlockchainStore.EFCore
{
    public abstract class BlockchainDbContextBase: DbContext
    {
        public string ColumnTypeForUnlimitedText { get; protected set; } = "nvarchar(max)";

        public DbSet<BlockProgress> BlockProgress { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<AddressTransaction> AddressTransactions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<TransactionLog> TransactionLogs { get; set; }
        public DbSet<TransactionVmStack> TransactionVmStacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BlockProgressEntityBuilder() { ColumnTypeForUnlimitedText = ColumnTypeForUnlimitedText });
            modelBuilder.ApplyConfiguration(new BlockEntityBuilder(){ColumnTypeForUnlimitedText = ColumnTypeForUnlimitedText});
            modelBuilder.ApplyConfiguration(new ContractEntityBuilder(){ColumnTypeForUnlimitedText = ColumnTypeForUnlimitedText});
            modelBuilder.ApplyConfiguration(new TransactionEntityBuilder(){ColumnTypeForUnlimitedText = ColumnTypeForUnlimitedText});
            modelBuilder.ApplyConfiguration(new TransactionLogEntityBuilder(){ColumnTypeForUnlimitedText = ColumnTypeForUnlimitedText});
            modelBuilder.ApplyConfiguration(new TransactionLogVmStackEntityBuilder(){ColumnTypeForUnlimitedText = ColumnTypeForUnlimitedText});
            modelBuilder.ApplyConfiguration(new AddressTransactionBuilder(){ColumnTypeForUnlimitedText = ColumnTypeForUnlimitedText});
            base.OnModelCreating(modelBuilder);
        }
    }
}
