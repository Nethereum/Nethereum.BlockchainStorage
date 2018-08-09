﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nethereum.BlockchainStore.SqlServer.Entities.Builders
{
    public class ContractModelBuilder:  BaseModelBuilder, IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> entityBuilder)
        {
            entityBuilder.ToTable("Contracts");
            entityBuilder.HasKey(m => m.RowIndex);

            entityBuilder.Property(m => m.Address).IsAddress();
            entityBuilder.Property(m => m.Name).HasMaxLength(255);
            entityBuilder.Property(m => m.ABI).IsNVarcharMax();
            entityBuilder.Property(m => m.Code).IsNVarcharMax();
            entityBuilder.Property(m => m.Creator).IsAddress();
            entityBuilder.Property(m => m.TransactionHash).IsHash();

            entityBuilder.HasIndex(m => m.Name);
            entityBuilder.HasIndex(m => m.Address);
        }
    }
}