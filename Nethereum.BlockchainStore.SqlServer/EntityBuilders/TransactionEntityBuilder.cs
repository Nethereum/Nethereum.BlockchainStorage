using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.SqlServer.EntityBuilders
{
    public class TransactionEntityBuilder : BaseEntityBuilder, IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> entityBuilder)
        {
            entityBuilder.ToTable("Transactions");
            entityBuilder.HasKey(b => b.RowIndex);

            entityBuilder.HasIndex(b => new {b.BlockNumber, b.Hash}).IsUnique();
            entityBuilder.HasIndex(b => b.Hash);
            entityBuilder.HasIndex(b => b.AddressFrom);
            entityBuilder.HasIndex(b => b.AddressTo);
            entityBuilder.HasIndex(b => b.NewContractAddress);

            entityBuilder.Property(t => t.BlockHash).IsHash();
            entityBuilder.Property(t => t.BlockNumber).IsBigInteger().IsRequired();
            entityBuilder.Property(b => b.Hash).IsHash().IsRequired();
            entityBuilder.Property(b => b.AddressFrom).IsAddress();
            entityBuilder.Property(b => b.AddressTo).IsAddress();
            entityBuilder.Property(b => b.Value).IsBigInteger();
            entityBuilder.Property(b => b.Input).IsNVarcharMax();
            entityBuilder.Property(b => b.ReceiptHash).IsHash();
            entityBuilder.Property(b => b.Error).IsNVarcharMax();
            entityBuilder.Property(b => b.NewContractAddress).IsAddress();
        }
    }
}