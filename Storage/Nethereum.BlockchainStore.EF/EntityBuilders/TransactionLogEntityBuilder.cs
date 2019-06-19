using System.Data.Entity.ModelConfiguration;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.EF.EntityBuilders
{
    public class TransactionLogEntityBuilder : EntityTypeConfiguration<TransactionLog>
    {
        public TransactionLogEntityBuilder()
        {
            ToTable("TransactionLogs");
            HasKey(m => m.RowIndex);

            Property(m => m.TransactionHash).IsHash().IsRequired();

            Property(m => m.Address).IsAddress();
            Property(m => m.EventHash).IsHash();
            Property(m => m.IndexVal1).IsHash();
            Property(m => m.IndexVal2).IsHash();
            Property(m => m.IndexVal3).IsHash();
            Property(m => m.Data).IsUnlimitedText();

            HasIndex(m => new { m.TransactionHash, m.LogIndex}).HasName("IX_TransactionLogs_TransactionHash_LogIndex").IsUnique();
            HasIndex(m => m.Address);
            HasIndex(m => m.EventHash);
            HasIndex(m => new {m.IndexVal1});
            HasIndex(m => new {m.IndexVal2});
            HasIndex(m => new {m.IndexVal3});
        }
    }
}
