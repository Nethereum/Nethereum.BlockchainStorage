using Nethereum.BlockchainStore.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Nethereum.BlockchainStore.EF.EntityBuilders
{
    public class AddressTransactionEntityBuilder : EntityTypeConfiguration<AddressTransaction>
    {        
        
        public AddressTransactionEntityBuilder()
        {
            ToTable("AddressTransactions");

            HasKey(b => b.RowIndex);

            HasIndex(b => new {b.BlockNumber, b.Hash, b.Address}).IsUnique().HasName("IX_Transactions_BlockNumber_Hash_Address");

            HasIndex(b => b.Hash);
            HasIndex(b => b.Address);
            Property(t => t.BlockNumber).IsBigInteger().IsRequired();
            Property(b => b.Hash).IsHash().IsRequired();
            Property(b => b.Address).IsAddress().IsRequired();
        }
    }
}
