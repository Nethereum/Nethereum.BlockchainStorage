﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nethereum.BlockchainStore.SqlServer.Entities.Builders
{
    public class TransactionLogModelBuilder : BaseModelBuilder, IEntityTypeConfiguration<TransactionLog>
    {
        public void Configure(EntityTypeBuilder<TransactionLog> entityBuilder)
        {
            entityBuilder.ToTable("TransactionLogs");
            entityBuilder.HasKey(m => m.RowIndex);

            entityBuilder.Property(m => m.TransactionHash).IsRequired();

            entityBuilder.Property(m => m.Address).IsAddress();
            entityBuilder.Property(m => m.Topics).IsNVarcharMax();
            entityBuilder.Property(m => m.Topic0).IsHash();
            entityBuilder.Property(m => m.Data).IsNVarcharMax();

            entityBuilder.HasIndex(m => new { m.TransactionHash, m.LogIndex}).IsUnique();
        }
    }
}
