﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nethereum.BlockchainStore.EFCore.SqlServer;

#nullable disable

namespace Nethereum.BlockchainStore.EFCore.SqlServer.Migrations.SqlServerBlockchainDbContext_goerliMigrations
{
    [DbContext(typeof(SqlServerBlockchainDbContext_goerli))]
    [Migration("20221014164544_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("goerli")
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Nethereum.BlockchainProcessing.BlockStorage.Entities.AddressTransaction", b =>
                {
                    b.Property<int>("RowIndex")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RowIndex"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(43)
                        .HasColumnType("nvarchar(43)");

                    b.Property<string>("BlockNumber")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasMaxLength(67)
                        .HasColumnType("nvarchar(67)");

                    b.Property<DateTime?>("RowCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RowUpdated")
                        .HasColumnType("datetime2");

                    b.HasKey("RowIndex");

                    b.HasIndex("Address");

                    b.HasIndex("Hash");

                    b.HasIndex("BlockNumber", "Hash", "Address")
                        .IsUnique();

                    b.ToTable("AddressTransactions", "goerli");
                });

            modelBuilder.Entity("Nethereum.BlockchainProcessing.BlockStorage.Entities.Block", b =>
                {
                    b.Property<int>("RowIndex")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RowIndex"), 1L, 1);

                    b.Property<string>("BaseFeePerGas")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("BlockNumber")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Difficulty")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ExtraData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GasLimit")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("GasUsed")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasMaxLength(67)
                        .HasColumnType("nvarchar(67)");

                    b.Property<string>("Miner")
                        .HasMaxLength(43)
                        .HasColumnType("nvarchar(43)");

                    b.Property<string>("Nonce")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ParentHash")
                        .IsRequired()
                        .HasMaxLength(67)
                        .HasColumnType("nvarchar(67)");

                    b.Property<DateTime?>("RowCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RowUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Size")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Timestamp")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("TotalDifficulty")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<long>("TransactionCount")
                        .HasColumnType("bigint");

                    b.HasKey("RowIndex");

                    b.HasIndex("BlockNumber", "Hash")
                        .IsUnique();

                    b.ToTable("Blocks", "goerli");
                });

            modelBuilder.Entity("Nethereum.BlockchainProcessing.BlockStorage.Entities.BlockProgress", b =>
                {
                    b.Property<int>("RowIndex")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RowIndex"), 1L, 1);

                    b.Property<string>("LastBlockProcessed")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("RowCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RowUpdated")
                        .HasColumnType("datetime2");

                    b.HasKey("RowIndex");

                    b.HasIndex("LastBlockProcessed");

                    b.ToTable("BlockProgress", "goerli");
                });

            modelBuilder.Entity("Nethereum.BlockchainProcessing.BlockStorage.Entities.Contract", b =>
                {
                    b.Property<int>("RowIndex")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RowIndex"), 1L, 1);

                    b.Property<string>("ABI")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Address")
                        .HasMaxLength(43)
                        .HasColumnType("nvarchar(43)");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Creator")
                        .HasMaxLength(43)
                        .HasColumnType("nvarchar(43)");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("RowCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RowUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("TransactionHash")
                        .HasMaxLength(67)
                        .HasColumnType("nvarchar(67)");

                    b.HasKey("RowIndex");

                    b.HasIndex("Address");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("Contracts", "goerli");
                });

            modelBuilder.Entity("Nethereum.BlockchainProcessing.BlockStorage.Entities.Transaction", b =>
                {
                    b.Property<int>("RowIndex")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RowIndex"), 1L, 1);

                    b.Property<string>("AddressFrom")
                        .HasMaxLength(43)
                        .HasColumnType("nvarchar(43)");

                    b.Property<string>("AddressTo")
                        .HasMaxLength(43)
                        .HasColumnType("nvarchar(43)");

                    b.Property<string>("BlockHash")
                        .HasMaxLength(67)
                        .HasColumnType("nvarchar(67)");

                    b.Property<string>("BlockNumber")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("CumulativeGasUsed")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("EffectiveGasPrice")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Error")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Failed")
                        .HasColumnType("bit");

                    b.Property<bool>("FailedCreateContract")
                        .HasColumnType("bit");

                    b.Property<string>("Gas")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("GasPrice")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("GasUsed")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("HasLog")
                        .HasColumnType("bit");

                    b.Property<bool>("HasVmStack")
                        .HasColumnType("bit");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasMaxLength(67)
                        .HasColumnType("nvarchar(67)");

                    b.Property<string>("Input")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MaxFeePerGas")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("MaxPriorityFeePerGas")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NewContractAddress")
                        .HasMaxLength(43)
                        .HasColumnType("nvarchar(43)");

                    b.Property<string>("Nonce")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ReceiptHash")
                        .HasMaxLength(67)
                        .HasColumnType("nvarchar(67)");

                    b.Property<DateTime?>("RowCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RowUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("TimeStamp")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("TransactionIndex")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Value")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("RowIndex");

                    b.HasIndex("AddressFrom");

                    b.HasIndex("AddressTo");

                    b.HasIndex("Hash");

                    b.HasIndex("NewContractAddress");

                    b.HasIndex("BlockNumber", "Hash")
                        .IsUnique();

                    b.ToTable("Transactions", "goerli");
                });

            modelBuilder.Entity("Nethereum.BlockchainProcessing.BlockStorage.Entities.TransactionLog", b =>
                {
                    b.Property<int>("RowIndex")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RowIndex"), 1L, 1);

                    b.Property<string>("Address")
                        .HasMaxLength(43)
                        .HasColumnType("nvarchar(43)");

                    b.Property<string>("Data")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EventHash")
                        .HasMaxLength(67)
                        .HasColumnType("nvarchar(67)");

                    b.Property<string>("IndexVal1")
                        .HasMaxLength(67)
                        .HasColumnType("nvarchar(67)");

                    b.Property<string>("IndexVal2")
                        .HasMaxLength(67)
                        .HasColumnType("nvarchar(67)");

                    b.Property<string>("IndexVal3")
                        .HasMaxLength(67)
                        .HasColumnType("nvarchar(67)");

                    b.Property<string>("LogIndex")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("RowCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RowUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("TransactionHash")
                        .IsRequired()
                        .HasMaxLength(67)
                        .HasColumnType("nvarchar(67)");

                    b.HasKey("RowIndex");

                    b.HasIndex("Address");

                    b.HasIndex("EventHash");

                    b.HasIndex("IndexVal1");

                    b.HasIndex("IndexVal2");

                    b.HasIndex("IndexVal3");

                    b.HasIndex("TransactionHash", "LogIndex")
                        .IsUnique()
                        .HasFilter("[LogIndex] IS NOT NULL");

                    b.ToTable("TransactionLogs", "goerli");
                });

            modelBuilder.Entity("Nethereum.BlockchainProcessing.BlockStorage.Entities.TransactionVmStack", b =>
                {
                    b.Property<int>("RowIndex")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RowIndex"), 1L, 1);

                    b.Property<string>("Address")
                        .HasMaxLength(43)
                        .HasColumnType("nvarchar(43)");

                    b.Property<DateTime?>("RowCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RowUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("StructLogs")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TransactionHash")
                        .HasMaxLength(67)
                        .HasColumnType("nvarchar(67)");

                    b.HasKey("RowIndex");

                    b.HasIndex("Address");

                    b.HasIndex("TransactionHash");

                    b.ToTable("TransactionLogVmStacks", "goerli");
                });
#pragma warning restore 612, 618
        }
    }
}
