﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nethereum.Blockchain.Sqlite;

namespace Nethereum.BlockchainStore.Sqlite.Migrations
{
    [DbContext(typeof(BlockchainDbContext))]
    partial class BlockchainDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846");

            modelBuilder.Entity("Nethereum.BlockchainStore.Entities.Block", b =>
                {
                    b.Property<int>("RowIndex")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BlockNumber")
                        .IsRequired()
                        .HasMaxLength(43);

                    b.Property<long>("Difficulty");

                    b.Property<string>("ExtraData");

                    b.Property<long>("GasLimit");

                    b.Property<long>("GasUsed");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasMaxLength(67);

                    b.Property<string>("Miner")
                        .HasMaxLength(43);

                    b.Property<long>("Nonce");

                    b.Property<string>("ParentHash")
                        .IsRequired()
                        .HasMaxLength(67);

                    b.Property<DateTime?>("RowCreated");

                    b.Property<DateTime?>("RowUpdated");

                    b.Property<long>("Size");

                    b.Property<long>("Timestamp");

                    b.Property<long>("TotalDifficulty");

                    b.Property<long>("TransactionCount");

                    b.HasKey("RowIndex");

                    b.HasIndex("BlockNumber", "Hash")
                        .IsUnique();

                    b.ToTable("Blocks");
                });

            modelBuilder.Entity("Nethereum.BlockchainStore.Entities.Contract", b =>
                {
                    b.Property<int>("RowIndex")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ABI")
                        .HasColumnType("TEXT");

                    b.Property<string>("Address")
                        .HasMaxLength(43);

                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<string>("Creator")
                        .HasMaxLength(43);

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<DateTime?>("RowCreated");

                    b.Property<DateTime?>("RowUpdated");

                    b.Property<string>("TransactionHash")
                        .HasMaxLength(67);

                    b.HasKey("RowIndex");

                    b.HasIndex("Address");

                    b.HasIndex("Name");

                    b.ToTable("Contracts");
                });

            modelBuilder.Entity("Nethereum.BlockchainStore.Entities.Transaction", b =>
                {
                    b.Property<int>("RowIndex")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddressFrom")
                        .HasMaxLength(43);

                    b.Property<string>("AddressTo")
                        .HasMaxLength(43);

                    b.Property<string>("BlockHash")
                        .HasMaxLength(67);

                    b.Property<string>("BlockNumber")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<long>("CumulativeGasUsed");

                    b.Property<string>("Error")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Failed");

                    b.Property<bool>("FailedCreateContract");

                    b.Property<long>("Gas");

                    b.Property<long>("GasPrice");

                    b.Property<long>("GasUsed");

                    b.Property<bool>("HasLog");

                    b.Property<bool>("HasVmStack");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasMaxLength(67);

                    b.Property<string>("Input")
                        .HasColumnType("TEXT");

                    b.Property<string>("NewContractAddress")
                        .HasMaxLength(43);

                    b.Property<long>("Nonce");

                    b.Property<string>("ReceiptHash")
                        .HasMaxLength(67);

                    b.Property<DateTime?>("RowCreated");

                    b.Property<DateTime?>("RowUpdated");

                    b.Property<long>("TimeStamp");

                    b.Property<long>("TransactionIndex");

                    b.Property<string>("Value")
                        .HasMaxLength(100);

                    b.HasKey("RowIndex");

                    b.HasIndex("AddressFrom");

                    b.HasIndex("AddressTo");

                    b.HasIndex("Hash");

                    b.HasIndex("NewContractAddress");

                    b.HasIndex("BlockNumber", "Hash")
                        .IsUnique();

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Nethereum.BlockchainStore.Entities.TransactionLog", b =>
                {
                    b.Property<int>("RowIndex")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .HasMaxLength(43);

                    b.Property<string>("Data")
                        .HasColumnType("TEXT");

                    b.Property<string>("EventHash")
                        .HasMaxLength(67);

                    b.Property<string>("IndexVal1")
                        .HasMaxLength(67);

                    b.Property<string>("IndexVal2")
                        .HasMaxLength(67);

                    b.Property<string>("IndexVal3")
                        .HasMaxLength(67);

                    b.Property<long>("LogIndex");

                    b.Property<DateTime?>("RowCreated");

                    b.Property<DateTime?>("RowUpdated");

                    b.Property<string>("TransactionHash")
                        .IsRequired();

                    b.HasKey("RowIndex");

                    b.HasIndex("Address");

                    b.HasIndex("EventHash");

                    b.HasIndex("IndexVal1");

                    b.HasIndex("IndexVal2");

                    b.HasIndex("IndexVal3");

                    b.HasIndex("TransactionHash", "LogIndex")
                        .IsUnique();

                    b.ToTable("TransactionLogs");
                });

            modelBuilder.Entity("Nethereum.BlockchainStore.Entities.TransactionVmStack", b =>
                {
                    b.Property<int>("RowIndex")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .HasMaxLength(43);

                    b.Property<DateTime?>("RowCreated");

                    b.Property<DateTime?>("RowUpdated");

                    b.Property<string>("StructLogs")
                        .HasColumnType("TEXT");

                    b.Property<string>("TransactionHash")
                        .HasMaxLength(67);

                    b.HasKey("RowIndex");

                    b.HasIndex("Address");

                    b.HasIndex("TransactionHash");

                    b.ToTable("TransactionLogVmStacks");
                });
#pragma warning restore 612, 618
        }
    }
}
