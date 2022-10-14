using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nethereum.BlockchainStore.EFCore.SqlServer.Migrations.SqlServerBlockchainDbContext_ropstenMigrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ropsten");

            migrationBuilder.CreateTable(
                name: "AddressTransactions",
                schema: "ropsten",
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
                schema: "ropsten",
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
                schema: "ropsten",
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
                schema: "ropsten",
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
                schema: "ropsten",
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
                schema: "ropsten",
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
                schema: "ropsten",
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
                schema: "ropsten",
                table: "AddressTransactions",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_AddressTransactions_BlockNumber_Hash_Address",
                schema: "ropsten",
                table: "AddressTransactions",
                columns: new[] { "BlockNumber", "Hash", "Address" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AddressTransactions_Hash",
                schema: "ropsten",
                table: "AddressTransactions",
                column: "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_BlockProgress_LastBlockProcessed",
                schema: "ropsten",
                table: "BlockProgress",
                column: "LastBlockProcessed");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_BlockNumber_Hash",
                schema: "ropsten",
                table: "Blocks",
                columns: new[] { "BlockNumber", "Hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_Address",
                schema: "ropsten",
                table: "Contracts",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_Name",
                schema: "ropsten",
                table: "Contracts",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_Address",
                schema: "ropsten",
                table: "TransactionLogs",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_EventHash",
                schema: "ropsten",
                table: "TransactionLogs",
                column: "EventHash");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_IndexVal1",
                schema: "ropsten",
                table: "TransactionLogs",
                column: "IndexVal1");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_IndexVal2",
                schema: "ropsten",
                table: "TransactionLogs",
                column: "IndexVal2");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_IndexVal3",
                schema: "ropsten",
                table: "TransactionLogs",
                column: "IndexVal3");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_TransactionHash_LogIndex",
                schema: "ropsten",
                table: "TransactionLogs",
                columns: new[] { "TransactionHash", "LogIndex" },
                unique: true,
                filter: "[LogIndex] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogVmStacks_Address",
                schema: "ropsten",
                table: "TransactionLogVmStacks",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogVmStacks_TransactionHash",
                schema: "ropsten",
                table: "TransactionLogVmStacks",
                column: "TransactionHash");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AddressFrom",
                schema: "ropsten",
                table: "Transactions",
                column: "AddressFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AddressTo",
                schema: "ropsten",
                table: "Transactions",
                column: "AddressTo");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BlockNumber_Hash",
                schema: "ropsten",
                table: "Transactions",
                columns: new[] { "BlockNumber", "Hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Hash",
                schema: "ropsten",
                table: "Transactions",
                column: "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_NewContractAddress",
                schema: "ropsten",
                table: "Transactions",
                column: "NewContractAddress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddressTransactions",
                schema: "ropsten");

            migrationBuilder.DropTable(
                name: "BlockProgress",
                schema: "ropsten");

            migrationBuilder.DropTable(
                name: "Blocks",
                schema: "ropsten");

            migrationBuilder.DropTable(
                name: "Contracts",
                schema: "ropsten");

            migrationBuilder.DropTable(
                name: "TransactionLogs",
                schema: "ropsten");

            migrationBuilder.DropTable(
                name: "TransactionLogVmStacks",
                schema: "ropsten");

            migrationBuilder.DropTable(
                name: "Transactions",
                schema: "ropsten");
        }
    }
}
