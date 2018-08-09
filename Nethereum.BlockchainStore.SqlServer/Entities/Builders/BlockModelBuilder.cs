using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nethereum.BlockchainStore.SqlServer.Entities;

namespace Nethereum.BlockchainStore.SqlServer
{
    public class BlockModelBuilder: BaseModelBuilder, IEntityTypeConfiguration<Block>
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