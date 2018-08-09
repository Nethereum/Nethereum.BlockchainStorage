using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nethereum.BlockchainStore.SqlServer.Entities;

namespace Nethereum.BlockchainStore.SqlServer
{
    public class AddressTransactionModelBuilder : BaseModelBuilder, IEntityTypeConfiguration<AddressTransaction>
    {
        public void Configure(EntityTypeBuilder<AddressTransaction> entityBuilder)
        {
            entityBuilder.ToTable("AddressTransactions");

            entityBuilder.HasKey(b => b.RowIndex);

            entityBuilder.Property(t => t.Address)
                .IsRequired()
                .IsAddress();

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

            entityBuilder.HasIndex(b => new {b.BlockNumber, b.Hash}).IsUnique();
            entityBuilder.HasIndex(t => t.Address);
        }
    }
}