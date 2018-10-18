IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF SCHEMA_ID(N'localhost') IS NULL EXEC(N'CREATE SCHEMA [localhost];');

GO

CREATE TABLE [localhost].[AddressTransactions] (
    [RowIndex] int NOT NULL IDENTITY,
    [RowCreated] datetime2 NULL,
    [RowUpdated] datetime2 NULL,
    [BlockNumber] nvarchar(100) NOT NULL,
    [Hash] nvarchar(67) NOT NULL,
    [Address] nvarchar(43) NOT NULL,
    CONSTRAINT [PK_AddressTransactions] PRIMARY KEY ([RowIndex])
);

GO

CREATE TABLE [localhost].[Blocks] (
    [RowIndex] int NOT NULL IDENTITY,
    [RowCreated] datetime2 NULL,
    [RowUpdated] datetime2 NULL,
    [BlockNumber] nvarchar(43) NOT NULL,
    [Hash] nvarchar(67) NOT NULL,
    [ParentHash] nvarchar(67) NOT NULL,
    [Nonce] bigint NOT NULL,
    [ExtraData] nvarchar(max) NULL,
    [Difficulty] bigint NOT NULL,
    [TotalDifficulty] bigint NOT NULL,
    [Size] bigint NOT NULL,
    [Miner] nvarchar(43) NULL,
    [GasLimit] bigint NOT NULL,
    [GasUsed] bigint NOT NULL,
    [Timestamp] bigint NOT NULL,
    [TransactionCount] bigint NOT NULL,
    CONSTRAINT [PK_Blocks] PRIMARY KEY ([RowIndex])
);

GO

CREATE TABLE [localhost].[Contracts] (
    [RowIndex] int NOT NULL IDENTITY,
    [RowCreated] datetime2 NULL,
    [RowUpdated] datetime2 NULL,
    [Address] nvarchar(43) NULL,
    [Name] nvarchar(255) NULL,
    [ABI] nvarchar(max) NULL,
    [Code] nvarchar(max) NULL,
    [Creator] nvarchar(43) NULL,
    [TransactionHash] nvarchar(67) NULL,
    CONSTRAINT [PK_Contracts] PRIMARY KEY ([RowIndex])
);

GO

CREATE TABLE [localhost].[TransactionLogs] (
    [RowIndex] int NOT NULL IDENTITY,
    [RowCreated] datetime2 NULL,
    [RowUpdated] datetime2 NULL,
    [TransactionHash] nvarchar(450) NOT NULL,
    [LogIndex] bigint NOT NULL,
    [Address] nvarchar(43) NULL,
    [EventHash] nvarchar(67) NULL,
    [IndexVal1] nvarchar(67) NULL,
    [IndexVal2] nvarchar(67) NULL,
    [IndexVal3] nvarchar(67) NULL,
    [Data] nvarchar(max) NULL,
    CONSTRAINT [PK_TransactionLogs] PRIMARY KEY ([RowIndex])
);

GO

CREATE TABLE [localhost].[TransactionLogVmStacks] (
    [RowIndex] int NOT NULL IDENTITY,
    [RowCreated] datetime2 NULL,
    [RowUpdated] datetime2 NULL,
    [Address] nvarchar(43) NULL,
    [TransactionHash] nvarchar(67) NULL,
    [StructLogs] nvarchar(max) NULL,
    CONSTRAINT [PK_TransactionLogVmStacks] PRIMARY KEY ([RowIndex])
);

GO

CREATE TABLE [localhost].[Transactions] (
    [RowIndex] int NOT NULL IDENTITY,
    [RowCreated] datetime2 NULL,
    [RowUpdated] datetime2 NULL,
    [BlockHash] nvarchar(67) NULL,
    [BlockNumber] nvarchar(100) NOT NULL,
    [Hash] nvarchar(67) NOT NULL,
    [AddressFrom] nvarchar(43) NULL,
    [TimeStamp] bigint NOT NULL,
    [TransactionIndex] bigint NOT NULL,
    [Value] nvarchar(100) NULL,
    [AddressTo] nvarchar(43) NULL,
    [Gas] bigint NOT NULL,
    [GasPrice] bigint NOT NULL,
    [Input] nvarchar(max) NULL,
    [Nonce] bigint NOT NULL,
    [Failed] bit NOT NULL,
    [ReceiptHash] nvarchar(67) NULL,
    [GasUsed] bigint NOT NULL,
    [CumulativeGasUsed] bigint NOT NULL,
    [HasLog] bit NOT NULL,
    [Error] nvarchar(max) NULL,
    [HasVmStack] bit NOT NULL,
    [NewContractAddress] nvarchar(43) NULL,
    [FailedCreateContract] bit NOT NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY ([RowIndex])
);

GO

CREATE INDEX [IX_AddressTransactions_Address] ON [localhost].[AddressTransactions] ([Address]);

GO

CREATE INDEX [IX_AddressTransactions_Hash] ON [localhost].[AddressTransactions] ([Hash]);

GO

CREATE UNIQUE INDEX [IX_AddressTransactions_BlockNumber_Hash_Address] ON [localhost].[AddressTransactions] ([BlockNumber], [Hash], [Address]);

GO

CREATE UNIQUE INDEX [IX_Blocks_BlockNumber_Hash] ON [localhost].[Blocks] ([BlockNumber], [Hash]);

GO

CREATE INDEX [IX_Contracts_Address] ON [localhost].[Contracts] ([Address]);

GO

CREATE INDEX [IX_Contracts_Name] ON [localhost].[Contracts] ([Name]);

GO

CREATE INDEX [IX_TransactionLogs_Address] ON [localhost].[TransactionLogs] ([Address]);

GO

CREATE INDEX [IX_TransactionLogs_EventHash] ON [localhost].[TransactionLogs] ([EventHash]);

GO

CREATE INDEX [IX_TransactionLogs_IndexVal1] ON [localhost].[TransactionLogs] ([IndexVal1]);

GO

CREATE INDEX [IX_TransactionLogs_IndexVal2] ON [localhost].[TransactionLogs] ([IndexVal2]);

GO

CREATE INDEX [IX_TransactionLogs_IndexVal3] ON [localhost].[TransactionLogs] ([IndexVal3]);

GO

CREATE UNIQUE INDEX [IX_TransactionLogs_TransactionHash_LogIndex] ON [localhost].[TransactionLogs] ([TransactionHash], [LogIndex]);

GO

CREATE INDEX [IX_TransactionLogVmStacks_Address] ON [localhost].[TransactionLogVmStacks] ([Address]);

GO

CREATE INDEX [IX_TransactionLogVmStacks_TransactionHash] ON [localhost].[TransactionLogVmStacks] ([TransactionHash]);

GO

CREATE INDEX [IX_Transactions_AddressFrom] ON [localhost].[Transactions] ([AddressFrom]);

GO

CREATE INDEX [IX_Transactions_AddressTo] ON [localhost].[Transactions] ([AddressTo]);

GO

CREATE INDEX [IX_Transactions_Hash] ON [localhost].[Transactions] ([Hash]);

GO

CREATE INDEX [IX_Transactions_NewContractAddress] ON [localhost].[Transactions] ([NewContractAddress]);

GO

CREATE UNIQUE INDEX [IX_Transactions_BlockNumber_Hash] ON [localhost].[Transactions] ([BlockNumber], [Hash]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20181010105726_InitialCreate', N'2.1.1-rtm-30846');

GO

