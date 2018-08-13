using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.EFCore.EntityBuilders
{
    public class TransactionLogEntityBuilder : BaseEntityBuilder, IEntityTypeConfiguration<TransactionLog>
    {
        public void Configure(EntityTypeBuilder<TransactionLog> entityBuilder)
        {
            entityBuilder.ToTable("TransactionLogs");
            entityBuilder.HasKey(m => m.RowIndex);

            entityBuilder.Property(m => m.TransactionHash).IsRequired();

            entityBuilder.Property(m => m.Address).IsAddress();
            entityBuilder.Property(m => m.Topics).IsUnlimitedText();
            entityBuilder.Property(m => m.Topic0).IsHash();
            entityBuilder.Property(m => m.Data).IsUnlimitedText();

            entityBuilder.HasIndex(m => new { m.TransactionHash, m.LogIndex}).IsUnique();
        }
    }
}
