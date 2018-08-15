﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.EFCore.EntityBuilders
{
    public class TransactionLogEntityBuilder : BaseEntityBuilder, IEntityTypeConfiguration<TransactionLog>
    {
        public void Configure(EntityTypeBuilder<TransactionLog> entityBuilder)
        {
            entityBuilder.ToTable("TransactionLogs");
            entityBuilder.HasKey(m => m.RowIndex);

            entityBuilder.Property(m => m.TransactionHash).IsRequired();

            entityBuilder.Property(m => m.Address).IsAddress();
            entityBuilder.Property(m => m.EventHash).IsHash();
            entityBuilder.Property(m => m.IndexVal1).IsHash();
            entityBuilder.Property(m => m.IndexVal2).IsHash();
            entityBuilder.Property(m => m.IndexVal3).IsHash();
            entityBuilder.Property(m => m.Data).IsUnlimitedText();

            entityBuilder.HasIndex(m => new { m.TransactionHash, m.LogIndex}).IsUnique();
            entityBuilder.HasIndex(m => m.Address);
            entityBuilder.HasIndex(m => m.EventHash);
            entityBuilder.HasIndex(m => new {m.IndexVal1});
            entityBuilder.HasIndex(m => new {m.IndexVal2});
            entityBuilder.HasIndex(m => new {m.IndexVal3});
        }
    }
}
