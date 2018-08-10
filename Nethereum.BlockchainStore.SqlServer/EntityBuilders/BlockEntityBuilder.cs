using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.SqlServer.EntityBuilders
{
    public class BlockEntityBuilder: BaseEntityBuilder, IEntityTypeConfiguration<Block>
    {
        public void Configure(EntityTypeBuilder<Block> entityBuilder)
        {
            entityBuilder.ToTable("Blocks");
            entityBuilder.HasKey(b => b.RowIndex);

            entityBuilder.Property(b => b.BlockNumber).IsAddress().IsRequired();
            entityBuilder.Property(b => b.Hash).IsHash().IsRequired();
            entityBuilder.Property(b => b.ParentHash).IsHash().IsRequired();
            entityBuilder.Property(b => b.Miner).IsAddress();

            entityBuilder.HasIndex(b => new {b.BlockNumber, b.Hash}).IsUnique();
        }
    }
}