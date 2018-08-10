USE [master]
GO
/*
IMPORTANT - PLEASE READ

This script creates a database called BlockchainStorage
It also creates users with preset usernames and passwords (these can be changed later)
Different ethereum networks are represented by different db schemas
So the tables are replicated for each schema
To avoid accidental data over writing - the users are each allocated a schema and they are not given access to other schemas

ALPHA!!
This is a one way script - if there are any errors it's going to leave things behind which will create other errors when it is retried

It was originally derived from scripting an Azure db - some of those settings may not be appropriate for other SQL db's.

Entity Framework Migrations
A migration initially created the schema that was then scripted.
However, as it stands the migration only creates tables for one specific schema.
This script creates tables for all of the schemas.

*/

CREATE LOGIN localhost1 WITH PASSWORD = 'MeLLfMA1wBlJCzSGZhkO';
GO
CREATE LOGIN ropsten1 WITH PASSWORD = 'MgOumZZ1LVbV5gSfen3M';
GO
CREATE LOGIN kovan1 WITH PASSWORD = 'm4753ct9kYMospex';
GO
CREATE LOGIN rinkeby1 WITH PASSWORD = 'rzNk9PyskZg0jLIl';
GO
CREATE LOGIN main1 WITH PASSWORD = 'ix2tjLkUEmPJ2Ah6';
GO
CREATE LOGIN blockchain_storage_admin WITH PASSWORD = 'eV5YMcv5aoDOsyEtJeCf'
GO

/****** Object:  Database [BlockchainStorage]    Script Date: 10/08/2018 14:50:42 ******/
CREATE DATABASE [BlockchainStorage]
GO
ALTER DATABASE [BlockchainStorage] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BlockchainStorage].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BlockchainStorage] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [BlockchainStorage] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [BlockchainStorage] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [BlockchainStorage] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [BlockchainStorage] SET ARITHABORT OFF 
GO
ALTER DATABASE [BlockchainStorage] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [BlockchainStorage] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [BlockchainStorage] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [BlockchainStorage] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [BlockchainStorage] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [BlockchainStorage] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [BlockchainStorage] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [BlockchainStorage] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [BlockchainStorage] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [BlockchainStorage] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [BlockchainStorage] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO
ALTER DATABASE [BlockchainStorage] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [BlockchainStorage] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [BlockchainStorage] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [BlockchainStorage] SET  MULTI_USER 
GO
ALTER DATABASE [BlockchainStorage] SET DB_CHAINING OFF 
GO
/*
--not supported by SQL Express
ALTER DATABASE [BlockchainStorage] SET ENCRYPTION ON
GO
*/
ALTER DATABASE [BlockchainStorage] SET QUERY_STORE = ON
GO
ALTER DATABASE [BlockchainStorage] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 100, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO)
GO
USE [BlockchainStorage]
GO
ALTER DATABASE SCOPED CONFIGURATION SET DISABLE_BATCH_MODE_ADAPTIVE_JOINS = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET DISABLE_BATCH_MODE_MEMORY_GRANT_FEEDBACK = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET DISABLE_INTERLEAVED_EXECUTION_TVF = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET ELEVATE_ONLINE = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET ELEVATE_RESUMABLE = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET IDENTITY_CACHE = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET ISOLATE_SECURITY_POLICY_CARDINALITY = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET OPTIMIZE_FOR_AD_HOC_WORKLOADS = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET XTP_PROCEDURE_EXECUTION_STATISTICS = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET XTP_QUERY_EXECUTION_STATISTICS = OFF;
GO
USE [BlockchainStorage]
GO
/****** Object:  User [ropsten1]    Script Date: 10/08/2018 14:50:42 ******/
CREATE USER [ropsten1] FOR LOGIN [ropsten1] WITH DEFAULT_SCHEMA=[ropsten]
GO
/****** Object:  User [rinkeby1]    Script Date: 10/08/2018 14:50:43 ******/
CREATE USER [rinkeby1] FOR LOGIN [rinkeby1] WITH DEFAULT_SCHEMA=[rinkeby]
GO
/****** Object:  User [main1]    Script Date: 10/08/2018 14:50:43 ******/
CREATE USER [main1] FOR LOGIN [main1] WITH DEFAULT_SCHEMA=[main]
GO
/****** Object:  User [localhost1]    Script Date: 10/08/2018 14:50:43 ******/
CREATE USER [localhost1] FOR LOGIN [localhost1] WITH DEFAULT_SCHEMA=[localhost]
GO
/****** Object:  User [kovan1]    Script Date: 10/08/2018 14:50:43 ******/
CREATE USER [kovan1] FOR LOGIN [kovan1] WITH DEFAULT_SCHEMA=[kovan]
GO
CREATE USER [blockchain_storage_admin] FOR LOGIN [blockchain_storage_admin] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  DatabaseRole [ropsten_users]    Script Date: 10/08/2018 14:50:43 ******/
CREATE ROLE [ropsten_users]
GO
/****** Object:  DatabaseRole [rinkeby_users]    Script Date: 10/08/2018 14:50:43 ******/
CREATE ROLE [rinkeby_users]
GO
/****** Object:  DatabaseRole [main_users]    Script Date: 10/08/2018 14:50:43 ******/
CREATE ROLE [main_users]
GO
/****** Object:  DatabaseRole [localhost_users]    Script Date: 10/08/2018 14:50:43 ******/
CREATE ROLE [localhost_users]
GO
/****** Object:  DatabaseRole [kovan_users]    Script Date: 10/08/2018 14:50:43 ******/
CREATE ROLE [kovan_users]
GO
ALTER ROLE [ropsten_users] ADD MEMBER [ropsten1]
GO
ALTER ROLE [rinkeby_users] ADD MEMBER [rinkeby1]
GO
ALTER ROLE [main_users] ADD MEMBER [main1]
GO
ALTER ROLE [localhost_users] ADD MEMBER [localhost1]
GO
ALTER ROLE [kovan_users] ADD MEMBER [kovan1]
GO
EXEC sp_addrolemember N'db_owner', N'blockchain_storage_admin'
GO
/****** Object:  Schema [kovan]    Script Date: 10/08/2018 14:50:43 ******/
CREATE SCHEMA [kovan]
GO
/****** Object:  Schema [localhost]    Script Date: 10/08/2018 14:50:43 ******/
CREATE SCHEMA [localhost]
GO
/****** Object:  Schema [main]    Script Date: 10/08/2018 14:50:43 ******/
CREATE SCHEMA [main]
GO
/****** Object:  Schema [rinkeby]    Script Date: 10/08/2018 14:50:44 ******/
CREATE SCHEMA [rinkeby]
GO
/****** Object:  Schema [ropsten]    Script Date: 10/08/2018 14:50:44 ******/
CREATE SCHEMA [ropsten]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 10/08/2018 14:50:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [localhost].[__EFMigrationsHistory]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [localhost].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--TABLES START HERE
/****** Object:  Table [localhost].[Blocks]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [localhost].[Blocks](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[BlockNumber] [nvarchar](43) NOT NULL,
	[Hash] [nvarchar](67) NOT NULL,
	[ParentHash] [nvarchar](67) NOT NULL,
	[Nonce] [bigint] NOT NULL,
	[ExtraData] [nvarchar](max) NULL,
	[Difficulty] [bigint] NOT NULL,
	[TotalDifficulty] [bigint] NOT NULL,
	[Size] [bigint] NOT NULL,
	[Miner] [nvarchar](43) NULL,
	[GasLimit] [bigint] NOT NULL,
	[GasUsed] [bigint] NOT NULL,
	[Timestamp] [bigint] NOT NULL,
	[TransactionCount] [bigint] NOT NULL,
 CONSTRAINT [PK_Blocks] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [localhost].[Contracts]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [localhost].[Contracts](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[Address] [nvarchar](43) NULL,
	[Name] [nvarchar](255) NULL,
	[ABI] [nvarchar](max) NULL,
	[Code] [nvarchar](max) NULL,
	[Creator] [nvarchar](43) NULL,
	[TransactionHash] [nvarchar](67) NULL,
 CONSTRAINT [PK_Contracts] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [localhost].[TransactionLogs]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [localhost].[TransactionLogs](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[TransactionHash] [nvarchar](450) NOT NULL,
	[LogIndex] [bigint] NOT NULL,
	[Address] [nvarchar](43) NULL,
	[Topics] [nvarchar](max) NULL,
	[Topic0] [nvarchar](67) NULL,
	[Data] [nvarchar](max) NULL,
 CONSTRAINT [PK_TransactionLogs] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [localhost].[TransactionLogVmStacks]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [localhost].[TransactionLogVmStacks](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[Address] [nvarchar](43) NULL,
	[TransactionHash] [nvarchar](67) NULL,
	[StructLogs] [nvarchar](max) NULL,
 CONSTRAINT [PK_TransactionLogVmStacks] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [localhost].[Transactions]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [localhost].[Transactions](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[BlockHash] [nvarchar](67) NULL,
	[BlockNumber] [nvarchar](100) NOT NULL,
	[Hash] [nvarchar](67) NOT NULL,
	[AddressFrom] [nvarchar](43) NULL,
	[TimeStamp] [bigint] NOT NULL,
	[TransactionIndex] [bigint] NOT NULL,
	[Value] [nvarchar](100) NULL,
	[AddressTo] [nvarchar](43) NULL,
	[Gas] [bigint] NOT NULL,
	[GasPrice] [bigint] NOT NULL,
	[Input] [nvarchar](max) NULL,
	[Nonce] [bigint] NOT NULL,
	[Failed] [bit] NOT NULL,
	[ReceiptHash] [nvarchar](67) NULL,
	[GasUsed] [bigint] NOT NULL,
	[CumulativeGasUsed] [bigint] NOT NULL,
	[HasLog] [bit] NOT NULL,
	[Error] [nvarchar](max) NULL,
	[HasVmStack] [bit] NOT NULL,
	[NewContractAddress] [nvarchar](43) NULL,
	[FailedCreateContract] [bit] NOT NULL,
 CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Blocks_BlockNumber_Hash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Blocks_BlockNumber_Hash] ON [localhost].[Blocks]
(
	[BlockNumber] ASC,
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Contracts_Address]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Contracts_Address] ON [localhost].[Contracts]
(
	[Address] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Contracts_Name]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Contracts_Name] ON [localhost].[Contracts]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TransactionLogs_TransactionHash_LogIndex]    Script Date: 10/08/2018 14:50:47 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_TransactionLogs_TransactionHash_LogIndex] ON [localhost].[TransactionLogs]
(
	[TransactionHash] ASC,
	[LogIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TransactionLogVmStacks_Address]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_TransactionLogVmStacks_Address] ON [localhost].[TransactionLogVmStacks]
(
	[Address] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TransactionLogVmStacks_TransactionHash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_TransactionLogVmStacks_TransactionHash] ON [localhost].[TransactionLogVmStacks]
(
	[TransactionHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_AddressFrom]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_AddressFrom] ON [localhost].[Transactions]
(
	[AddressFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_AddressTo]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_AddressTo] ON [localhost].[Transactions]
(
	[AddressTo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_BlockNumber_Hash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Transactions_BlockNumber_Hash] ON [localhost].[Transactions]
(
	[BlockNumber] ASC,
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_Hash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_Hash] ON [localhost].[Transactions]
(
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_NewContractAddress]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_NewContractAddress] ON [localhost].[Transactions]
(
	[NewContractAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
-- TABLES END HERE

--start main
--TABLES START HERE
/****** Object:  Table [main].[Blocks]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [main].[Blocks](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[BlockNumber] [nvarchar](43) NOT NULL,
	[Hash] [nvarchar](67) NOT NULL,
	[ParentHash] [nvarchar](67) NOT NULL,
	[Nonce] [bigint] NOT NULL,
	[ExtraData] [nvarchar](max) NULL,
	[Difficulty] [bigint] NOT NULL,
	[TotalDifficulty] [bigint] NOT NULL,
	[Size] [bigint] NOT NULL,
	[Miner] [nvarchar](43) NULL,
	[GasLimit] [bigint] NOT NULL,
	[GasUsed] [bigint] NOT NULL,
	[Timestamp] [bigint] NOT NULL,
	[TransactionCount] [bigint] NOT NULL,
 CONSTRAINT [PK_Blocks] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [main].[Contracts]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [main].[Contracts](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[Address] [nvarchar](43) NULL,
	[Name] [nvarchar](255) NULL,
	[ABI] [nvarchar](max) NULL,
	[Code] [nvarchar](max) NULL,
	[Creator] [nvarchar](43) NULL,
	[TransactionHash] [nvarchar](67) NULL,
 CONSTRAINT [PK_Contracts] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [main].[TransactionLogs]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [main].[TransactionLogs](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[TransactionHash] [nvarchar](450) NOT NULL,
	[LogIndex] [bigint] NOT NULL,
	[Address] [nvarchar](43) NULL,
	[Topics] [nvarchar](max) NULL,
	[Topic0] [nvarchar](67) NULL,
	[Data] [nvarchar](max) NULL,
 CONSTRAINT [PK_TransactionLogs] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [main].[TransactionLogVmStacks]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [main].[TransactionLogVmStacks](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[Address] [nvarchar](43) NULL,
	[TransactionHash] [nvarchar](67) NULL,
	[StructLogs] [nvarchar](max) NULL,
 CONSTRAINT [PK_TransactionLogVmStacks] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [main].[Transactions]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [main].[Transactions](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[BlockHash] [nvarchar](67) NULL,
	[BlockNumber] [nvarchar](100) NOT NULL,
	[Hash] [nvarchar](67) NOT NULL,
	[AddressFrom] [nvarchar](43) NULL,
	[TimeStamp] [bigint] NOT NULL,
	[TransactionIndex] [bigint] NOT NULL,
	[Value] [nvarchar](100) NULL,
	[AddressTo] [nvarchar](43) NULL,
	[Gas] [bigint] NOT NULL,
	[GasPrice] [bigint] NOT NULL,
	[Input] [nvarchar](max) NULL,
	[Nonce] [bigint] NOT NULL,
	[Failed] [bit] NOT NULL,
	[ReceiptHash] [nvarchar](67) NULL,
	[GasUsed] [bigint] NOT NULL,
	[CumulativeGasUsed] [bigint] NOT NULL,
	[HasLog] [bit] NOT NULL,
	[Error] [nvarchar](max) NULL,
	[HasVmStack] [bit] NOT NULL,
	[NewContractAddress] [nvarchar](43) NULL,
	[FailedCreateContract] [bit] NOT NULL,
 CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Blocks_BlockNumber_Hash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Blocks_BlockNumber_Hash] ON [main].[Blocks]
(
	[BlockNumber] ASC,
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Contracts_Address]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Contracts_Address] ON [main].[Contracts]
(
	[Address] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Contracts_Name]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Contracts_Name] ON [main].[Contracts]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TransactionLogs_TransactionHash_LogIndex]    Script Date: 10/08/2018 14:50:47 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_TransactionLogs_TransactionHash_LogIndex] ON [main].[TransactionLogs]
(
	[TransactionHash] ASC,
	[LogIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TransactionLogVmStacks_Address]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_TransactionLogVmStacks_Address] ON [main].[TransactionLogVmStacks]
(
	[Address] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TransactionLogVmStacks_TransactionHash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_TransactionLogVmStacks_TransactionHash] ON [main].[TransactionLogVmStacks]
(
	[TransactionHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_AddressFrom]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_AddressFrom] ON [main].[Transactions]
(
	[AddressFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_AddressTo]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_AddressTo] ON [main].[Transactions]
(
	[AddressTo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_BlockNumber_Hash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Transactions_BlockNumber_Hash] ON [main].[Transactions]
(
	[BlockNumber] ASC,
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_Hash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_Hash] ON [main].[Transactions]
(
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_NewContractAddress]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_NewContractAddress] ON [main].[Transactions]
(
	[NewContractAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
-- TABLES END HERE
--end main

--start kovan
--TABLES START HERE
/****** Object:  Table [kovan].[Blocks]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [kovan].[Blocks](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[BlockNumber] [nvarchar](43) NOT NULL,
	[Hash] [nvarchar](67) NOT NULL,
	[ParentHash] [nvarchar](67) NOT NULL,
	[Nonce] [bigint] NOT NULL,
	[ExtraData] [nvarchar](max) NULL,
	[Difficulty] [bigint] NOT NULL,
	[TotalDifficulty] [bigint] NOT NULL,
	[Size] [bigint] NOT NULL,
	[Miner] [nvarchar](43) NULL,
	[GasLimit] [bigint] NOT NULL,
	[GasUsed] [bigint] NOT NULL,
	[Timestamp] [bigint] NOT NULL,
	[TransactionCount] [bigint] NOT NULL,
 CONSTRAINT [PK_Blocks] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [kovan].[Contracts]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [kovan].[Contracts](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[Address] [nvarchar](43) NULL,
	[Name] [nvarchar](255) NULL,
	[ABI] [nvarchar](max) NULL,
	[Code] [nvarchar](max) NULL,
	[Creator] [nvarchar](43) NULL,
	[TransactionHash] [nvarchar](67) NULL,
 CONSTRAINT [PK_Contracts] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [kovan].[TransactionLogs]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [kovan].[TransactionLogs](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[TransactionHash] [nvarchar](450) NOT NULL,
	[LogIndex] [bigint] NOT NULL,
	[Address] [nvarchar](43) NULL,
	[Topics] [nvarchar](max) NULL,
	[Topic0] [nvarchar](67) NULL,
	[Data] [nvarchar](max) NULL,
 CONSTRAINT [PK_TransactionLogs] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [kovan].[TransactionLogVmStacks]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [kovan].[TransactionLogVmStacks](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[Address] [nvarchar](43) NULL,
	[TransactionHash] [nvarchar](67) NULL,
	[StructLogs] [nvarchar](max) NULL,
 CONSTRAINT [PK_TransactionLogVmStacks] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [kovan].[Transactions]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [kovan].[Transactions](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[BlockHash] [nvarchar](67) NULL,
	[BlockNumber] [nvarchar](100) NOT NULL,
	[Hash] [nvarchar](67) NOT NULL,
	[AddressFrom] [nvarchar](43) NULL,
	[TimeStamp] [bigint] NOT NULL,
	[TransactionIndex] [bigint] NOT NULL,
	[Value] [nvarchar](100) NULL,
	[AddressTo] [nvarchar](43) NULL,
	[Gas] [bigint] NOT NULL,
	[GasPrice] [bigint] NOT NULL,
	[Input] [nvarchar](max) NULL,
	[Nonce] [bigint] NOT NULL,
	[Failed] [bit] NOT NULL,
	[ReceiptHash] [nvarchar](67) NULL,
	[GasUsed] [bigint] NOT NULL,
	[CumulativeGasUsed] [bigint] NOT NULL,
	[HasLog] [bit] NOT NULL,
	[Error] [nvarchar](max) NULL,
	[HasVmStack] [bit] NOT NULL,
	[NewContractAddress] [nvarchar](43) NULL,
	[FailedCreateContract] [bit] NOT NULL,
 CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Blocks_BlockNumber_Hash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Blocks_BlockNumber_Hash] ON [kovan].[Blocks]
(
	[BlockNumber] ASC,
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Contracts_Address]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Contracts_Address] ON [kovan].[Contracts]
(
	[Address] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Contracts_Name]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Contracts_Name] ON [kovan].[Contracts]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TransactionLogs_TransactionHash_LogIndex]    Script Date: 10/08/2018 14:50:47 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_TransactionLogs_TransactionHash_LogIndex] ON [kovan].[TransactionLogs]
(
	[TransactionHash] ASC,
	[LogIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TransactionLogVmStacks_Address]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_TransactionLogVmStacks_Address] ON [kovan].[TransactionLogVmStacks]
(
	[Address] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TransactionLogVmStacks_TransactionHash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_TransactionLogVmStacks_TransactionHash] ON [kovan].[TransactionLogVmStacks]
(
	[TransactionHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_AddressFrom]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_AddressFrom] ON [kovan].[Transactions]
(
	[AddressFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_AddressTo]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_AddressTo] ON [kovan].[Transactions]
(
	[AddressTo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_BlockNumber_Hash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Transactions_BlockNumber_Hash] ON [kovan].[Transactions]
(
	[BlockNumber] ASC,
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_Hash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_Hash] ON [kovan].[Transactions]
(
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_NewContractAddress]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_NewContractAddress] ON [kovan].[Transactions]
(
	[NewContractAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
-- TABLES END HERE
--end kovan

--start rinkeby
--TABLES START HERE
/****** Object:  Table [rinkeby].[Blocks]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [rinkeby].[Blocks](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[BlockNumber] [nvarchar](43) NOT NULL,
	[Hash] [nvarchar](67) NOT NULL,
	[ParentHash] [nvarchar](67) NOT NULL,
	[Nonce] [bigint] NOT NULL,
	[ExtraData] [nvarchar](max) NULL,
	[Difficulty] [bigint] NOT NULL,
	[TotalDifficulty] [bigint] NOT NULL,
	[Size] [bigint] NOT NULL,
	[Miner] [nvarchar](43) NULL,
	[GasLimit] [bigint] NOT NULL,
	[GasUsed] [bigint] NOT NULL,
	[Timestamp] [bigint] NOT NULL,
	[TransactionCount] [bigint] NOT NULL,
 CONSTRAINT [PK_Blocks] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [rinkeby].[Contracts]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [rinkeby].[Contracts](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[Address] [nvarchar](43) NULL,
	[Name] [nvarchar](255) NULL,
	[ABI] [nvarchar](max) NULL,
	[Code] [nvarchar](max) NULL,
	[Creator] [nvarchar](43) NULL,
	[TransactionHash] [nvarchar](67) NULL,
 CONSTRAINT [PK_Contracts] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [rinkeby].[TransactionLogs]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [rinkeby].[TransactionLogs](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[TransactionHash] [nvarchar](450) NOT NULL,
	[LogIndex] [bigint] NOT NULL,
	[Address] [nvarchar](43) NULL,
	[Topics] [nvarchar](max) NULL,
	[Topic0] [nvarchar](67) NULL,
	[Data] [nvarchar](max) NULL,
 CONSTRAINT [PK_TransactionLogs] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [rinkeby].[TransactionLogVmStacks]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [rinkeby].[TransactionLogVmStacks](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[Address] [nvarchar](43) NULL,
	[TransactionHash] [nvarchar](67) NULL,
	[StructLogs] [nvarchar](max) NULL,
 CONSTRAINT [PK_TransactionLogVmStacks] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [rinkeby].[Transactions]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [rinkeby].[Transactions](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[BlockHash] [nvarchar](67) NULL,
	[BlockNumber] [nvarchar](100) NOT NULL,
	[Hash] [nvarchar](67) NOT NULL,
	[AddressFrom] [nvarchar](43) NULL,
	[TimeStamp] [bigint] NOT NULL,
	[TransactionIndex] [bigint] NOT NULL,
	[Value] [nvarchar](100) NULL,
	[AddressTo] [nvarchar](43) NULL,
	[Gas] [bigint] NOT NULL,
	[GasPrice] [bigint] NOT NULL,
	[Input] [nvarchar](max) NULL,
	[Nonce] [bigint] NOT NULL,
	[Failed] [bit] NOT NULL,
	[ReceiptHash] [nvarchar](67) NULL,
	[GasUsed] [bigint] NOT NULL,
	[CumulativeGasUsed] [bigint] NOT NULL,
	[HasLog] [bit] NOT NULL,
	[Error] [nvarchar](max) NULL,
	[HasVmStack] [bit] NOT NULL,
	[NewContractAddress] [nvarchar](43) NULL,
	[FailedCreateContract] [bit] NOT NULL,
 CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Blocks_BlockNumber_Hash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Blocks_BlockNumber_Hash] ON [rinkeby].[Blocks]
(
	[BlockNumber] ASC,
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Contracts_Address]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Contracts_Address] ON [rinkeby].[Contracts]
(
	[Address] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Contracts_Name]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Contracts_Name] ON [rinkeby].[Contracts]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TransactionLogs_TransactionHash_LogIndex]    Script Date: 10/08/2018 14:50:47 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_TransactionLogs_TransactionHash_LogIndex] ON [rinkeby].[TransactionLogs]
(
	[TransactionHash] ASC,
	[LogIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TransactionLogVmStacks_Address]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_TransactionLogVmStacks_Address] ON [rinkeby].[TransactionLogVmStacks]
(
	[Address] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TransactionLogVmStacks_TransactionHash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_TransactionLogVmStacks_TransactionHash] ON [rinkeby].[TransactionLogVmStacks]
(
	[TransactionHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_AddressFrom]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_AddressFrom] ON [rinkeby].[Transactions]
(
	[AddressFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_AddressTo]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_AddressTo] ON [rinkeby].[Transactions]
(
	[AddressTo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_BlockNumber_Hash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Transactions_BlockNumber_Hash] ON [rinkeby].[Transactions]
(
	[BlockNumber] ASC,
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_Hash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_Hash] ON [rinkeby].[Transactions]
(
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_NewContractAddress]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_NewContractAddress] ON [rinkeby].[Transactions]
(
	[NewContractAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
-- TABLES END HERE
--end rinkeby

--start ropsten
--TABLES START HERE
/****** Object:  Table [ropsten].[Blocks]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ropsten].[Blocks](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[BlockNumber] [nvarchar](43) NOT NULL,
	[Hash] [nvarchar](67) NOT NULL,
	[ParentHash] [nvarchar](67) NOT NULL,
	[Nonce] [bigint] NOT NULL,
	[ExtraData] [nvarchar](max) NULL,
	[Difficulty] [bigint] NOT NULL,
	[TotalDifficulty] [bigint] NOT NULL,
	[Size] [bigint] NOT NULL,
	[Miner] [nvarchar](43) NULL,
	[GasLimit] [bigint] NOT NULL,
	[GasUsed] [bigint] NOT NULL,
	[Timestamp] [bigint] NOT NULL,
	[TransactionCount] [bigint] NOT NULL,
 CONSTRAINT [PK_Blocks] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [ropsten].[Contracts]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ropsten].[Contracts](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[Address] [nvarchar](43) NULL,
	[Name] [nvarchar](255) NULL,
	[ABI] [nvarchar](max) NULL,
	[Code] [nvarchar](max) NULL,
	[Creator] [nvarchar](43) NULL,
	[TransactionHash] [nvarchar](67) NULL,
 CONSTRAINT [PK_Contracts] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [ropsten].[TransactionLogs]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ropsten].[TransactionLogs](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[TransactionHash] [nvarchar](450) NOT NULL,
	[LogIndex] [bigint] NOT NULL,
	[Address] [nvarchar](43) NULL,
	[Topics] [nvarchar](max) NULL,
	[Topic0] [nvarchar](67) NULL,
	[Data] [nvarchar](max) NULL,
 CONSTRAINT [PK_TransactionLogs] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [ropsten].[TransactionLogVmStacks]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ropsten].[TransactionLogVmStacks](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[Address] [nvarchar](43) NULL,
	[TransactionHash] [nvarchar](67) NULL,
	[StructLogs] [nvarchar](max) NULL,
 CONSTRAINT [PK_TransactionLogVmStacks] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [ropsten].[Transactions]    Script Date: 10/08/2018 14:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ropsten].[Transactions](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[RowCreated] [datetime2](7) NULL,
	[RowUpdated] [datetime2](7) NULL,
	[BlockHash] [nvarchar](67) NULL,
	[BlockNumber] [nvarchar](100) NOT NULL,
	[Hash] [nvarchar](67) NOT NULL,
	[AddressFrom] [nvarchar](43) NULL,
	[TimeStamp] [bigint] NOT NULL,
	[TransactionIndex] [bigint] NOT NULL,
	[Value] [nvarchar](100) NULL,
	[AddressTo] [nvarchar](43) NULL,
	[Gas] [bigint] NOT NULL,
	[GasPrice] [bigint] NOT NULL,
	[Input] [nvarchar](max) NULL,
	[Nonce] [bigint] NOT NULL,
	[Failed] [bit] NOT NULL,
	[ReceiptHash] [nvarchar](67) NULL,
	[GasUsed] [bigint] NOT NULL,
	[CumulativeGasUsed] [bigint] NOT NULL,
	[HasLog] [bit] NOT NULL,
	[Error] [nvarchar](max) NULL,
	[HasVmStack] [bit] NOT NULL,
	[NewContractAddress] [nvarchar](43) NULL,
	[FailedCreateContract] [bit] NOT NULL,
 CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Blocks_BlockNumber_Hash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Blocks_BlockNumber_Hash] ON [ropsten].[Blocks]
(
	[BlockNumber] ASC,
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Contracts_Address]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Contracts_Address] ON [ropsten].[Contracts]
(
	[Address] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Contracts_Name]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Contracts_Name] ON [ropsten].[Contracts]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TransactionLogs_TransactionHash_LogIndex]    Script Date: 10/08/2018 14:50:47 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_TransactionLogs_TransactionHash_LogIndex] ON [ropsten].[TransactionLogs]
(
	[TransactionHash] ASC,
	[LogIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TransactionLogVmStacks_Address]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_TransactionLogVmStacks_Address] ON [ropsten].[TransactionLogVmStacks]
(
	[Address] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TransactionLogVmStacks_TransactionHash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_TransactionLogVmStacks_TransactionHash] ON [ropsten].[TransactionLogVmStacks]
(
	[TransactionHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_AddressFrom]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_AddressFrom] ON [ropsten].[Transactions]
(
	[AddressFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_AddressTo]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_AddressTo] ON [ropsten].[Transactions]
(
	[AddressTo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_BlockNumber_Hash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Transactions_BlockNumber_Hash] ON [ropsten].[Transactions]
(
	[BlockNumber] ASC,
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_Hash]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_Hash] ON [ropsten].[Transactions]
(
	[Hash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Transactions_NewContractAddress]    Script Date: 10/08/2018 14:50:47 ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_NewContractAddress] ON [ropsten].[Transactions]
(
	[NewContractAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
-- TABLES END HERE
--end ropsten


USE [master]
GO
ALTER DATABASE [BlockchainStorage] SET  READ_WRITE 
GO

GRANT ALTER ON SCHEMA :: localhost TO localhost_users;  
GO
GRANT INSERT ON SCHEMA :: localhost TO localhost_users;  
GO
GRANT SELECT ON SCHEMA :: localhost TO localhost_users;  
GO
GRANT UPDATE ON SCHEMA :: localhost TO localhost_users;  
GO
GRANT DELETE ON SCHEMA :: localhost TO localhost_users;  
GO
GRANT EXECUTE ON SCHEMA :: localhost TO localhost_users;  
GO

GRANT ALTER ON SCHEMA :: kovan TO kovan_users;  
GO
GRANT INSERT ON SCHEMA :: kovan TO kovan_users;  
GO
GRANT SELECT ON SCHEMA :: kovan TO kovan_users;  
GO
GRANT UPDATE ON SCHEMA :: kovan TO kovan_users;  
GO
GRANT DELETE ON SCHEMA :: kovan TO kovan_users;  
GO
GRANT EXECUTE ON SCHEMA :: kovan TO kovan_users;  
GO

GRANT ALTER ON SCHEMA :: main TO main_users;  
GO
GRANT INSERT ON SCHEMA :: main TO main_users;  
GO
GRANT SELECT ON SCHEMA :: main TO main_users;  
GO
GRANT UPDATE ON SCHEMA :: main TO main_users;  
GO
GRANT DELETE ON SCHEMA :: main TO main_users;  
GO
GRANT EXECUTE ON SCHEMA :: main TO main_users;  
GO

GRANT ALTER ON SCHEMA :: rinkeby TO rinkeby_users;  
GO
GRANT INSERT ON SCHEMA :: rinkeby TO rinkeby_users;  
GO
GRANT SELECT ON SCHEMA :: rinkeby TO rinkeby_users;  
GO
GRANT UPDATE ON SCHEMA :: rinkeby TO rinkeby_users;  
GO
GRANT DELETE ON SCHEMA :: rinkeby TO rinkeby_users;  
GO
GRANT EXECUTE ON SCHEMA :: rinkeby TO rinkeby_users;  
GO

GRANT ALTER ON SCHEMA :: ropsten TO ropsten_users;  
GO
GRANT INSERT ON SCHEMA :: ropsten TO ropsten_users;  
GO
GRANT SELECT ON SCHEMA :: ropsten TO ropsten_users;  
GO
GRANT UPDATE ON SCHEMA :: ropsten TO ropsten_users;  
GO
GRANT DELETE ON SCHEMA :: ropsten TO ropsten_users;  
GO
GRANT EXECUTE ON SCHEMA :: ropsten TO ropsten_users;  
GO