using System.Data.Entity.ModelConfiguration;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.EF.EntityBuilders
{
    public class BlockEntityBuilder: EntityTypeConfiguration<Block>
    {
        public BlockEntityBuilder()
        {
            ToTable("Blocks");
            HasKey(b => b.RowIndex);

            Property(b => b.BlockNumber).IsAddress().IsRequired();
            Property(b => b.Hash).IsHash().IsRequired();
            Property(b => b.ParentHash).IsHash().IsRequired();
            Property(b => b.Miner).IsAddress();
            HasIndex(b => new {b.BlockNumber, b.Hash}).IsUnique();                
        }
    }
}