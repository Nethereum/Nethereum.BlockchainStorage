using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Nethereum.BlockchainStore.EF.EntityBuilders
{
    public class TransactionLogVmStackEntityBuilder : EntityTypeConfiguration<TransactionVmStack>
    {
        public TransactionLogVmStackEntityBuilder()
        {
            ToTable("TransactionLogVmStacks");
            HasKey(m => m.RowIndex);

            Property(m => m.Address).IsAddress();
            Property(m => m.TransactionHash).IsHash();
            Property(m => m.StructLogs).IsUnlimitedText();

            HasIndex(m => m.Address);
            HasIndex(m => m.TransactionHash);
        }
    }
}
