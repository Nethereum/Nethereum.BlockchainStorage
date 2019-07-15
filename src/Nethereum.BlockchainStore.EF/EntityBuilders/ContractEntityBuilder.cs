using Nethereum.BlockchainProcessing.Storage.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Nethereum.BlockchainStore.EF.EntityBuilders
{
    public class ContractEntityBuilder:  EntityTypeConfiguration<Contract>
    {
        public ContractEntityBuilder()
        {
            ToTable("Contracts");
            HasKey(m => m.RowIndex);

            Property(m => m.Address).IsAddress();
            Property(m => m.Name).HasMaxLength(255);
            Property(m => m.ABI).IsUnlimitedText();
            Property(m => m.Code).IsUnlimitedText();
            Property(m => m.Creator).IsAddress();
            Property(m => m.TransactionHash).IsHash();

            HasIndex(m => m.Name);
            HasIndex(m => m.Address).IsUnique();
        }
    }
}
