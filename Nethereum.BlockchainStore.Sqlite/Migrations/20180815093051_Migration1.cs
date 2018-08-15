using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nethereum.BlockchainStore.Sqlite.Migrations
{
    public partial class Migration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blocks",
                columns: table => new
                {
                    RowIndex = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
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
                columns: table => new
                {
                    RowIndex = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RowCreated = table.Column<DateTime>(nullable: true),
                    RowUpdated = table.Column<DateTime>(nullable: true),
                    Address = table.Column<string>(maxLength: 43, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    ABI = table.Column<string>(type: "TEXT", nullable: true),
                    Code = table.Column<string>(type: "TEXT", nullable: true),
                    Creator = table.Column<string>(maxLength: 43, nullable: true),
                    TransactionHash = table.Column<string>(maxLength: 67, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.RowIndex);
                });

            migrationBuilder.CreateTable(
                name: "TransactionLogs",
                columns: table => new
                {
                    RowIndex = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RowCreated = table.Column<DateTime>(nullable: true),
                    RowUpdated = table.Column<DateTime>(nullable: true),
                    TransactionHash = table.Column<string>(nullable: false),
                    LogIndex = table.Column<long>(nullable: false),
                    Address = table.Column<string>(maxLength: 43, nullable: true),
                    EventHash = table.Column<string>(maxLength: 67, nullable: true),
                    IndexVal1 = table.Column<string>(maxLength: 67, nullable: true),
                    IndexVal2 = table.Column<string>(maxLength: 67, nullable: true),
                    IndexVal3 = table.Column<string>(maxLength: 67, nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLogs", x => x.RowIndex);
                });

            migrationBuilder.CreateTable(
                name: "TransactionLogVmStacks",
                columns: table => new
                {
                    RowIndex = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RowCreated = table.Column<DateTime>(nullable: true),
                    RowUpdated = table.Column<DateTime>(nullable: true),
                    Address = table.Column<string>(maxLength: 43, nullable: true),
                    TransactionHash = table.Column<string>(maxLength: 67, nullable: true),
                    StructLogs = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLogVmStacks", x => x.RowIndex);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    RowIndex = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
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
                    Input = table.Column<string>(type: "TEXT", nullable: true),
                    Nonce = table.Column<long>(nullable: false),
                    Failed = table.Column<bool>(nullable: false),
                    ReceiptHash = table.Column<string>(maxLength: 67, nullable: true),
                    GasUsed = table.Column<long>(nullable: false),
                    CumulativeGasUsed = table.Column<long>(nullable: false),
                    HasLog = table.Column<bool>(nullable: false),
                    Error = table.Column<string>(type: "TEXT", nullable: true),
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
                table: "Blocks",
                columns: new[] { "BlockNumber", "Hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_Address",
                table: "Contracts",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_Name",
                table: "Contracts",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_Address",
                table: "TransactionLogs",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_EventHash",
                table: "TransactionLogs",
                column: "EventHash");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_IndexVal1",
                table: "TransactionLogs",
                column: "IndexVal1");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_IndexVal2",
                table: "TransactionLogs",
                column: "IndexVal2");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_IndexVal3",
                table: "TransactionLogs",
                column: "IndexVal3");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_TransactionHash_LogIndex",
                table: "TransactionLogs",
                columns: new[] { "TransactionHash", "LogIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogVmStacks_Address",
                table: "TransactionLogVmStacks",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogVmStacks_TransactionHash",
                table: "TransactionLogVmStacks",
                column: "TransactionHash");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AddressFrom",
                table: "Transactions",
                column: "AddressFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AddressTo",
                table: "Transactions",
                column: "AddressTo");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Hash",
                table: "Transactions",
                column: "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_NewContractAddress",
                table: "Transactions",
                column: "NewContractAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BlockNumber_Hash",
                table: "Transactions",
                columns: new[] { "BlockNumber", "Hash" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blocks");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "TransactionLogs");

            migrationBuilder.DropTable(
                name: "TransactionLogVmStacks");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
