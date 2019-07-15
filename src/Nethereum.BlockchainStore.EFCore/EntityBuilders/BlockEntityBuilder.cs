using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nethereum.BlockchainProcessing.Storage.Entities;

namespace Nethereum.BlockchainStore.EFCore.EntityBuilders
{
    public class BlockEntityBuilder: BaseEntityBuilder, IEntityTypeConfiguration<Block>
    {
        public void Configure(EntityTypeBuilder<Block> entityBuilder)
        {
            entityBuilder.ToTable("Blocks");
            entityBuilder.HasKey(b => b.RowIndex);

            entityBuilder.Property(b => b.BlockNumber).IsBigInteger().IsRequired();
            entityBuilder.Property(b => b.Hash).IsHash().IsRequired();
            entityBuilder.Property(b => b.ParentHash).IsHash().IsRequired();
            entityBuilder.Property(b => b.Miner).IsAddress();

            entityBuilder.Property(b => b.Difficulty).IsBigInteger();
            entityBuilder.Property(b => b.TotalDifficulty).IsBigInteger();
            entityBuilder.Property(b => b.Size).IsBigInteger();
            entityBuilder.Property(b => b.GasLimit).IsBigInteger();
            entityBuilder.Property(b => b.GasUsed).IsBigInteger();
            entityBuilder.Property(b => b.Timestamp).IsBigInteger();
            entityBuilder.Property(b => b.Nonce).IsBigInteger();

            entityBuilder.HasIndex(b => new {b.BlockNumber, b.Hash}).IsUnique();
        }
    }
}