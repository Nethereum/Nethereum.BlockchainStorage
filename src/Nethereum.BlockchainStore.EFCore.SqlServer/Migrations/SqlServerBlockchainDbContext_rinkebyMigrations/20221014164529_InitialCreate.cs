﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nethereum.BlockchainStore.EFCore.SqlServer.Migrations.SqlServerBlockchainDbContext_rinkebyMigrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "rinkeby");

            migrationBuilder.CreateTable(
                name: "AddressTransactions",
                schema: "rinkeby",
                columns: table => new
                {
                    RowIndex = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlockNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(67)", maxLength: 67, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(43)", maxLength: 43, nullable: false),
                    RowCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressTransactions", x => x.RowIndex);
                });

            migrationBuilder.CreateTable(
                name: "BlockProgress",
                schema: "rinkeby",
                columns: table => new
                {
                    RowIndex = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastBlockProcessed = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RowCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockProgress", x => x.RowIndex);
                });

            migrationBuilder.CreateTable(
                name: "Blocks",
                schema: "rinkeby",
                columns: table => new
                {
                    RowIndex = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlockNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(67)", maxLength: 67, nullable: false),
                    ParentHash = table.Column<string>(type: "nvarchar(67)", maxLength: 67, nullable: false),
                    Nonce = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExtraData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Difficulty = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TotalDifficulty = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Size = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Miner = table.Column<string>(type: "nvarchar(43)", maxLength: 43, nullable: true),
                    GasLimit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GasUsed = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Timestamp = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransactionCount = table.Column<long>(type: "bigint", nullable: false),
                    BaseFeePerGas = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RowCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blocks", x => x.RowIndex);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                schema: "rinkeby",
                columns: table => new
                {
                    RowIndex = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(43)", maxLength: 43, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ABI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Creator = table.Column<string>(type: "nvarchar(43)", maxLength: 43, nullable: true),
                    TransactionHash = table.Column<string>(type: "nvarchar(67)", maxLength: 67, nullable: true),
                    RowCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.RowIndex);
                });

            migrationBuilder.CreateTable(
                name: "TransactionLogs",
                schema: "rinkeby",
                columns: table => new
                {
                    RowIndex = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionHash = table.Column<string>(type: "nvarchar(67)", maxLength: 67, nullable: false),
                    LogIndex = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(43)", maxLength: 43, nullable: true),
                    EventHash = table.Column<string>(type: "nvarchar(67)", maxLength: 67, nullable: true),
                    IndexVal1 = table.Column<string>(type: "nvarchar(67)", maxLength: 67, nullable: true),
                    IndexVal2 = table.Column<string>(type: "nvarchar(67)", maxLength: 67, nullable: true),
                    IndexVal3 = table.Column<string>(type: "nvarchar(67)", maxLength: 67, nullable: true),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLogs", x => x.RowIndex);
                });

            migrationBuilder.CreateTable(
                name: "TransactionLogVmStacks",
                schema: "rinkeby",
                columns: table => new
                {
                    RowIndex = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(43)", maxLength: 43, nullable: true),
                    TransactionHash = table.Column<string>(type: "nvarchar(67)", maxLength: 67, nullable: true),
                    StructLogs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLogVmStacks", x => x.RowIndex);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                schema: "rinkeby",
                columns: table => new
                {
                    RowIndex = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RowCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BlockHash = table.Column<string>(type: "nvarchar(67)", maxLength: 67, nullable: true),
                    BlockNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(67)", maxLength: 67, nullable: false),
                    AddressFrom = table.Column<string>(type: "nvarchar(43)", maxLength: 43, nullable: true),
                    TimeStamp = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransactionIndex = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AddressTo = table.Column<string>(type: "nvarchar(43)", maxLength: 43, nullable: true),
                    Gas = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GasPrice = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Input = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nonce = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Failed = table.Column<bool>(type: "bit", nullable: false),
                    ReceiptHash = table.Column<string>(type: "nvarchar(67)", maxLength: 67, nullable: true),
                    GasUsed = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CumulativeGasUsed = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EffectiveGasPrice = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HasLog = table.Column<bool>(type: "bit", nullable: false),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasVmStack = table.Column<bool>(type: "bit", nullable: false),
                    NewContractAddress = table.Column<string>(type: "nvarchar(43)", maxLength: 43, nullable: true),
                    FailedCreateContract = table.Column<bool>(type: "bit", nullable: false),
                    MaxFeePerGas = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaxPriorityFeePerGas = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.RowIndex);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddressTransactions_Address",
                schema: "rinkeby",
                table: "AddressTransactions",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_AddressTransactions_BlockNumber_Hash_Address",
                schema: "rinkeby",
                table: "AddressTransactions",
                columns: new[] { "BlockNumber", "Hash", "Address" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AddressTransactions_Hash",
                schema: "rinkeby",
                table: "AddressTransactions",
                column: "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_BlockProgress_LastBlockProcessed",
                schema: "rinkeby",
                table: "BlockProgress",
                column: "LastBlockProcessed");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_BlockNumber_Hash",
                schema: "rinkeby",
                table: "Blocks",
                columns: new[] { "BlockNumber", "Hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_Address",
                schema: "rinkeby",
                table: "Contracts",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_Name",
                schema: "rinkeby",
                table: "Contracts",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_Address",
                schema: "rinkeby",
                table: "TransactionLogs",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_EventHash",
                schema: "rinkeby",
                table: "TransactionLogs",
                column: "EventHash");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_IndexVal1",
                schema: "rinkeby",
                table: "TransactionLogs",
                column: "IndexVal1");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_IndexVal2",
                schema: "rinkeby",
                table: "TransactionLogs",
                column: "IndexVal2");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_IndexVal3",
                schema: "rinkeby",
                table: "TransactionLogs",
                column: "IndexVal3");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_TransactionHash_LogIndex",
                schema: "rinkeby",
                table: "TransactionLogs",
                columns: new[] { "TransactionHash", "LogIndex" },
                unique: true,
                filter: "[LogIndex] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogVmStacks_Address",
                schema: "rinkeby",
                table: "TransactionLogVmStacks",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogVmStacks_TransactionHash",
                schema: "rinkeby",
                table: "TransactionLogVmStacks",
                column: "TransactionHash");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AddressFrom",
                schema: "rinkeby",
                table: "Transactions",
                column: "AddressFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AddressTo",
                schema: "rinkeby",
                table: "Transactions",
                column: "AddressTo");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BlockNumber_Hash",
                schema: "rinkeby",
                table: "Transactions",
                columns: new[] { "BlockNumber", "Hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Hash",
                schema: "rinkeby",
                table: "Transactions",
                column: "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_NewContractAddress",
                schema: "rinkeby",
                table: "Transactions",
                column: "NewContractAddress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddressTransactions",
                schema: "rinkeby");

            migrationBuilder.DropTable(
                name: "BlockProgress",
                schema: "rinkeby");

            migrationBuilder.DropTable(
                name: "Blocks",
                schema: "rinkeby");

            migrationBuilder.DropTable(
                name: "Contracts",
                schema: "rinkeby");

            migrationBuilder.DropTable(
                name: "TransactionLogs",
                schema: "rinkeby");

            migrationBuilder.DropTable(
                name: "TransactionLogVmStacks",
                schema: "rinkeby");

            migrationBuilder.DropTable(
                name: "Transactions",
                schema: "rinkeby");
        }
    }
}
