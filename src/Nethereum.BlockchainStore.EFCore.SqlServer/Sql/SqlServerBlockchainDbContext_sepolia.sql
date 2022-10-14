IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF SCHEMA_ID(N'sepolia') IS NULL EXEC(N'CREATE SCHEMA [sepolia];');
GO

CREATE TABLE [sepolia].[AddressTransactions] (
    [RowIndex] int NOT NULL IDENTITY,
    [BlockNumber] nvarchar(100) NOT NULL,
    [Hash] nvarchar(67) NOT NULL,
    [Address] nvarchar(43) NOT NULL,
    [RowCreated] datetime2 NULL,
    [RowUpdated] datetime2 NULL,
    CONSTRAINT [PK_AddressTransactions] PRIMARY KEY ([RowIndex])
);
GO

CREATE TABLE [sepolia].[BlockProgress] (
    [RowIndex] int NOT NULL IDENTITY,
    [LastBlockProcessed] nvarchar(100) NOT NULL,
    [RowCreated] datetime2 NULL,
    [RowUpdated] datetime2 NULL,
    CONSTRAINT [PK_BlockProgress] PRIMARY KEY ([RowIndex])
);
GO

CREATE TABLE [sepolia].[Blocks] (
    [RowIndex] int NOT NULL IDENTITY,
    [BlockNumber] nvarchar(100) NOT NULL,
    [Hash] nvarchar(67) NOT NULL,
    [ParentHash] nvarchar(67) NOT NULL,
    [Nonce] nvarchar(100) NULL,
    [ExtraData] nvarchar(max) NULL,
    [Difficulty] nvarchar(100) NULL,
    [TotalDifficulty] nvarchar(100) NULL,
    [Size] nvarchar(100) NULL,
    [Miner] nvarchar(43) NULL,
    [GasLimit] nvarchar(100) NULL,
    [GasUsed] nvarchar(100) NULL,
    [Timestamp] nvarchar(100) NULL,
    [TransactionCount] bigint NOT NULL,
    [BaseFeePerGas] nvarchar(100) NULL,
    [RowCreated] datetime2 NULL,
    [RowUpdated] datetime2 NULL,
    CONSTRAINT [PK_Blocks] PRIMARY KEY ([RowIndex])
);
GO

CREATE TABLE [sepolia].[Contracts] (
    [RowIndex] int NOT NULL IDENTITY,
    [Address] nvarchar(43) NULL,
    [Name] nvarchar(255) NULL,
    [ABI] nvarchar(max) NULL,
    [Code] nvarchar(max) NULL,
    [Creator] nvarchar(43) NULL,
    [TransactionHash] nvarchar(67) NULL,
    [RowCreated] datetime2 NULL,
    [RowUpdated] datetime2 NULL,
    CONSTRAINT [PK_Contracts] PRIMARY KEY ([RowIndex])
);
GO

CREATE TABLE [sepolia].[TransactionLogs] (
    [RowIndex] int NOT NULL IDENTITY,
    [TransactionHash] nvarchar(67) NOT NULL,
    [LogIndex] nvarchar(100) NULL,
    [Address] nvarchar(43) NULL,
    [EventHash] nvarchar(67) NULL,
    [IndexVal1] nvarchar(67) NULL,
    [IndexVal2] nvarchar(67) NULL,
    [IndexVal3] nvarchar(67) NULL,
    [Data] nvarchar(max) NULL,
    [RowCreated] datetime2 NULL,
    [RowUpdated] datetime2 NULL,
    CONSTRAINT [PK_TransactionLogs] PRIMARY KEY ([RowIndex])
);
GO

CREATE TABLE [sepolia].[TransactionLogVmStacks] (
    [RowIndex] int NOT NULL IDENTITY,
    [Address] nvarchar(43) NULL,
    [TransactionHash] nvarchar(67) NULL,
    [StructLogs] nvarchar(max) NULL,
    [RowCreated] datetime2 NULL,
    [RowUpdated] datetime2 NULL,
    CONSTRAINT [PK_TransactionLogVmStacks] PRIMARY KEY ([RowIndex])
);
GO

CREATE TABLE [sepolia].[Transactions] (
    [RowIndex] int NOT NULL IDENTITY,
    [RowCreated] datetime2 NULL,
    [RowUpdated] datetime2 NULL,
    [BlockHash] nvarchar(67) NULL,
    [BlockNumber] nvarchar(100) NOT NULL,
    [Hash] nvarchar(67) NOT NULL,
    [AddressFrom] nvarchar(43) NULL,
    [TimeStamp] nvarchar(100) NULL,
    [TransactionIndex] nvarchar(100) NULL,
    [Value] nvarchar(100) NULL,
    [AddressTo] nvarchar(43) NULL,
    [Gas] nvarchar(100) NULL,
    [GasPrice] nvarchar(100) NULL,
    [Input] nvarchar(max) NULL,
    [Nonce] nvarchar(100) NULL,
    [Failed] bit NOT NULL,
    [ReceiptHash] nvarchar(67) NULL,
    [GasUsed] nvarchar(100) NULL,
    [CumulativeGasUsed] nvarchar(100) NULL,
    [EffectiveGasPrice] nvarchar(100) NULL,
    [HasLog] bit NOT NULL,
    [Error] nvarchar(max) NULL,
    [HasVmStack] bit NOT NULL,
    [NewContractAddress] nvarchar(43) NULL,
    [FailedCreateContract] bit NOT NULL,
    [MaxFeePerGas] nvarchar(100) NULL,
    [MaxPriorityFeePerGas] nvarchar(100) NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY ([RowIndex])
);
GO

CREATE INDEX [IX_AddressTransactions_Address] ON [sepolia].[AddressTransactions] ([Address]);
GO

CREATE UNIQUE INDEX [IX_AddressTransactions_BlockNumber_Hash_Address] ON [sepolia].[AddressTransactions] ([BlockNumber], [Hash], [Address]);
GO

CREATE INDEX [IX_AddressTransactions_Hash] ON [sepolia].[AddressTransactions] ([Hash]);
GO

CREATE INDEX [IX_BlockProgress_LastBlockProcessed] ON [sepolia].[BlockProgress] ([LastBlockProcessed]);
GO

CREATE UNIQUE INDEX [IX_Blocks_BlockNumber_Hash] ON [sepolia].[Blocks] ([BlockNumber], [Hash]);
GO

CREATE INDEX [IX_Contracts_Address] ON [sepolia].[Contracts] ([Address]);
GO

CREATE UNIQUE INDEX [IX_Contracts_Name] ON [sepolia].[Contracts] ([Name]) WHERE [Name] IS NOT NULL;
GO

CREATE INDEX [IX_TransactionLogs_Address] ON [sepolia].[TransactionLogs] ([Address]);
GO

CREATE INDEX [IX_TransactionLogs_EventHash] ON [sepolia].[TransactionLogs] ([EventHash]);
GO

CREATE INDEX [IX_TransactionLogs_IndexVal1] ON [sepolia].[TransactionLogs] ([IndexVal1]);
GO

CREATE INDEX [IX_TransactionLogs_IndexVal2] ON [sepolia].[TransactionLogs] ([IndexVal2]);
GO

CREATE INDEX [IX_TransactionLogs_IndexVal3] ON [sepolia].[TransactionLogs] ([IndexVal3]);
GO

CREATE UNIQUE INDEX [IX_TransactionLogs_TransactionHash_LogIndex] ON [sepolia].[TransactionLogs] ([TransactionHash], [LogIndex]) WHERE [LogIndex] IS NOT NULL;
GO

CREATE INDEX [IX_TransactionLogVmStacks_Address] ON [sepolia].[TransactionLogVmStacks] ([Address]);
GO

CREATE INDEX [IX_TransactionLogVmStacks_TransactionHash] ON [sepolia].[TransactionLogVmStacks] ([TransactionHash]);
GO

CREATE INDEX [IX_Transactions_AddressFrom] ON [sepolia].[Transactions] ([AddressFrom]);
GO

CREATE INDEX [IX_Transactions_AddressTo] ON [sepolia].[Transactions] ([AddressTo]);
GO

CREATE UNIQUE INDEX [IX_Transactions_BlockNumber_Hash] ON [sepolia].[Transactions] ([BlockNumber], [Hash]);
GO

CREATE INDEX [IX_Transactions_Hash] ON [sepolia].[Transactions] ([Hash]);
GO

CREATE INDEX [IX_Transactions_NewContractAddress] ON [sepolia].[Transactions] ([NewContractAddress]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20221014164550_InitialCreate', N'6.0.10');
GO

COMMIT;
GO

