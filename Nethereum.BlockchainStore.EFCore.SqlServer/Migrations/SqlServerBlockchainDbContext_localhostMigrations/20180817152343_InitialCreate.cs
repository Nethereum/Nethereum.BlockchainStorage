﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nethereum.BlockchainStore.EFCore.SqlServer.Migrations.SqlServerBlockchainDbContext_localhostMigrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "localhost");

            migrationBuilder.CreateTable(
                name: "Blocks",
                schema: "localhost",
                columns: table => new
                {
                    RowIndex = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowCreated = table.Column<DateTime>(nullable: true),
                    RowUpdated = table.Column<DateTime>(nullable: true),
                    BlockNumber = table.Column<string>(maxLength: 43, nullable: false),
                    Hash = table.Column<string>(maxLength: 67, nullable: false),
                    ParentHash = table.Column<string>(maxLength: 67, nullable: false),
                    Nonce = table.Column<long>(nullable: false),
                    ExtraData = table.Column<string>(nullable: true),
                    Difficulty = table.Column<long>(nullable: false),
                    TotalDifficulty = table.Column<long>(nullable: false),
                    Size = table.Column<long>(nullable: false),
                    Miner = table.Column<string>(maxLength: 43, nullable: true),
                    GasLimit = table.Column<long>(nullable: false),
                    GasUsed = table.Column<long>(nullable: false),
                    Timestamp = table.Column<long>(nullable: false),
                    TransactionCount = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blocks", x => x.RowIndex);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                schema: "localhost",
                columns: table => new
                {
                    RowIndex = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowCreated = table.Column<DateTime>(nullable: true),
                    RowUpdated = table.Column<DateTime>(nullable: true),
                    Address = table.Column<string>(maxLength: 43, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    ABI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Creator = table.Column<string>(maxLength: 43, nullable: true),
                    TransactionHash = table.Column<string>(maxLength: 67, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.RowIndex);
                });

            migrationBuilder.CreateTable(
                name: "TransactionLogs",
                schema: "localhost",
                columns: table => new
                {
                    RowIndex = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowCreated = table.Column<DateTime>(nullable: true),
                    RowUpdated = table.Column<DateTime>(nullable: true),
                    TransactionHash = table.Column<string>(nullable: false),
                    LogIndex = table.Column<long>(nullable: false),
                    Address = table.Column<string>(maxLength: 43, nullable: true),
                    EventHash = table.Column<string>(maxLength: 67, nullable: true),
                    IndexVal1 = table.Column<string>(maxLength: 67, nullable: true),
                    IndexVal2 = table.Column<string>(maxLength: 67, nullable: true),
                    IndexVal3 = table.Column<string>(maxLength: 67, nullable: true),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLogs", x => x.RowIndex);
                });

            migrationBuilder.CreateTable(
                name: "TransactionLogVmStacks",
                schema: "localhost",
                columns: table => new
                {
                    RowIndex = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowCreated = table.Column<DateTime>(nullable: true),
                    RowUpdated = table.Column<DateTime>(nullable: true),
                    Address = table.Column<string>(maxLength: 43, nullable: true),
                    TransactionHash = table.Column<string>(maxLength: 67, nullable: true),
                    StructLogs = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLogVmStacks", x => x.RowIndex);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                schema: "localhost",
                columns: table => new
                {
                    RowIndex = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowCreated = table.Column<DateTime>(nullable: true),
                    RowUpdated = table.Column<DateTime>(nullable: true),
                    BlockHash = table.Column<string>(maxLength: 67, nullable: true),
                    BlockNumber = table.Column<string>(maxLength: 100, nullable: false),
                    Hash = table.Column<string>(maxLength: 67, nullable: false),
                    AddressFrom = table.Column<string>(maxLength: 43, nullable: true),
                    TimeStamp = table.Column<long>(nullable: false),
                    TransactionIndex = table.Column<long>(nullable: false),
                    Value = table.Column<string>(maxLength: 100, nullable: true),
                    AddressTo = table.Column<string>(maxLength: 43, nullable: true),
                    Gas = table.Column<long>(nullable: false),
                    GasPrice = table.Column<long>(nullable: false),
                    Input = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nonce = table.Column<long>(nullable: false),
                    Failed = table.Column<bool>(nullable: false),
                    ReceiptHash = table.Column<string>(maxLength: 67, nullable: true),
                    GasUsed = table.Column<long>(nullable: false),
                    CumulativeGasUsed = table.Column<long>(nullable: false),
                    HasLog = table.Column<bool>(nullable: false),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasVmStack = table.Column<bool>(nullable: false),
                    NewContractAddress = table.Column<string>(maxLength: 43, nullable: true),
                    FailedCreateContract = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.RowIndex);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_BlockNumber_Hash",
                schema: "localhost",
                table: "Blocks",
                columns: new[] { "BlockNumber", "Hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_Address",
                schema: "localhost",
                table: "Contracts",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_Name",
                schema: "localhost",
                table: "Contracts",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_Address",
                schema: "localhost",
                table: "TransactionLogs",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_EventHash",
                schema: "localhost",
                table: "TransactionLogs",
                column: "EventHash");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_IndexVal1",
                schema: "localhost",
                table: "TransactionLogs",
                column: "IndexVal1");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_IndexVal2",
                schema: "localhost",
                table: "TransactionLogs",
                column: "IndexVal2");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_IndexVal3",
                schema: "localhost",
                table: "TransactionLogs",
                column: "IndexVal3");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_TransactionHash_LogIndex",
                schema: "localhost",
                table: "TransactionLogs",
                columns: new[] { "TransactionHash", "LogIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogVmStacks_Address",
                schema: "localhost",
                table: "TransactionLogVmStacks",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogVmStacks_TransactionHash",
                schema: "localhost",
                table: "TransactionLogVmStacks",
                column: "TransactionHash");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AddressFrom",
                schema: "localhost",
                table: "Transactions",
                column: "AddressFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AddressTo",
                schema: "localhost",
                table: "Transactions",
                column: "AddressTo");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Hash",
                schema: "localhost",
                table: "Transactions",
                column: "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_NewContractAddress",
                schema: "localhost",
                table: "Transactions",
                column: "NewContractAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BlockNumber_Hash",
                schema: "localhost",
                table: "Transactions",
                columns: new[] { "BlockNumber", "Hash" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blocks",
                schema: "localhost");

            migrationBuilder.DropTable(
                name: "Contracts",
                schema: "localhost");

            migrationBuilder.DropTable(
                name: "TransactionLogs",
                schema: "localhost");

            migrationBuilder.DropTable(
                name: "TransactionLogVmStacks",
                schema: "localhost");

            migrationBuilder.DropTable(
                name: "Transactions",
                schema: "localhost");
        }
    }
}
