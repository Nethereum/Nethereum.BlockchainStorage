using System.Data.Entity.ModelConfiguration;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.EF.EntityBuilders
{
    public class TransactionEntityBuilder : EntityTypeConfiguration<Transaction>
    {
        public TransactionEntityBuilder()
        {
            ToTable("Transactions");
            HasKey(b => b.RowIndex);

            HasIndex(b => new {b.BlockNumber, b.Hash}).IsUnique();
            HasIndex(b => b.Hash);
            HasIndex(b => b.AddressFrom);
            HasIndex(b => b.AddressTo);
            HasIndex(b => b.NewContractAddress);

            Property(t => t.BlockHash).IsHash();
            Property(t => t.BlockNumber).IsBigInteger().IsRequired();
            Property(b => b.Hash).IsHash().IsRequired();
            Property(b => b.AddressFrom).IsAddress();
            Property(b => b.AddressTo).IsAddress();
            Property(b => b.Value).IsBigInteger();
            Property(b => b.Input).IsUnlimitedText();
            Property(b => b.ReceiptHash).IsHash();
            Property(b => b.Error).IsUnlimitedText();
            Property(b => b.NewContractAddress).IsAddress();
        }
    }
}