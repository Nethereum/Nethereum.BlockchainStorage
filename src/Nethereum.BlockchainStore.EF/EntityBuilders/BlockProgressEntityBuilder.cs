using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Nethereum.BlockchainStore.EF.EntityBuilders
{
    public class BlockProgressEntityBuilder : EntityTypeConfiguration<BlockProgress>
    {
        public BlockProgressEntityBuilder()
        {
            ToTable("BlockProgress");
            HasKey(b => b.RowIndex);

            Property(b => b.LastBlockProcessed).IsAddress().IsRequired();
            HasIndex(b => b.LastBlockProcessed).HasName("IX_BlockProgress_LastBlockProcessed");                
        }
    }
}