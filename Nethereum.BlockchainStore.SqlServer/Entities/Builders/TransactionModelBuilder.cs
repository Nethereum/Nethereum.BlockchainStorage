using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nethereum.BlockchainStore.SqlServer.Entities;

namespace Nethereum.BlockchainStore.SqlServer
{
    public class TransactionModelBuilder : BaseModelBuilder, IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> _entityBuilder)
        {
            _entityBuilder.ToTable("Transactions");
            _entityBuilder.HasKey(b => b.RowIndex);

            _entityBuilder.HasIndex(b => new {b.BlockNumber, b.Hash}).IsUnique();

            _entityBuilder.Property(t => t.BlockHash).IsHash();
            _entityBuilder.Property(t => t.BlockNumber).IsBigInteger().IsRequired();
            _entityBuilder.Property(b => b.Hash).IsHash().IsRequired();
            _entityBuilder.Property(b => b.AddressFrom).IsAddress();
            _entityBuilder.Property(b => b.AddressTo).IsAddress();
            _entityBuilder.Property(b => b.Value).IsBigInteger();
            _entityBuilder.Property(b => b.Input).IsNVarcharMax();
            _entityBuilder.Property(b => b.ReceiptHash).IsHash();
            _entityBuilder.Property(b => b.Error).IsNVarcharMax();
            _entityBuilder.Property(b => b.NewContractAddress).IsAddress();
        }
    }
}