using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nethereum.BlockchainStore.SqlServer.Entities.Builders
{
    public class TransactionLogVmStackModelBuilder : BaseModelBuilder, IEntityTypeConfiguration<TransactionVmStack>
    {
        public void Configure(EntityTypeBuilder<TransactionVmStack> entityBuilder)
        {
            entityBuilder.ToTable("TransactionLogVmStacks");
            entityBuilder.HasKey(m => m.RowIndex);

            entityBuilder.Property(m => m.Address).IsAddress();
            entityBuilder.Property(m => m.TransactionHash).IsHash();
            entityBuilder.Property(m => m.StructLogs).IsNVarcharMax();

            entityBuilder.HasIndex(m => m.Address);
            entityBuilder.HasIndex(m => m.TransactionHash);
        }
    }
}
