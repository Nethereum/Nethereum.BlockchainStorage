--address = 43
--hash = 67

CREATE TABLE localhost.AddressTransactions
(
	RowIndex int identity primary key,
	BlockHash nvarchar(100),
	[Hash] nvarchar(100),
	AddressFrom nvarchar(100),
	[TimeStamp] bigint,
	[TransactionIndex] bigint,
	[Value] nvarchar(max),
	[Address] nvarchar(100),
	AddressTo nvarchar(100),
	BlockNumber nvarchar(100),
	Gas bigint,
	GasPrice bigint,
	Input nvarchar(max),
	Nonce bigint,
	Failed bit,
	ReceiptHash nvarchar(100),
	GasUsed bigint,
	CumulativeGasUsed bigint,
	HasLog bit,
	Error nvarchar(max),
	HasVmStack bit,
	NewContractAddress nvarchar(100),
	FailedCreateContract bit
)

CREATE TABLE localhost.Transactions
(
	RowIndex int identity primary key,
	BlockHash nvarchar(100),
	[Hash] nvarchar(100),
	AddressFrom nvarchar(100),
	[TimeStamp] bigint,
	[TransactionIndex] bigint,
	[Value] nvarchar(max),
	AddressTo nvarchar(100),
	BlockNumber nvarchar(100),
	Gas bigint,
	GasPrice bigint,
	Input nvarchar(max),
	Nonce bigint,
	Failed bit,
	ReceiptHash nvarchar(100),
	GasUsed bigint,
	CumulativeGasUsed bigint,
	HasLog bit,
	Error nvarchar(max),
	HasVmStack bit,
	NewContractAddress nvarchar(100),
	FailedCreateContract bit
)

CREATE TABLE localhost.[Blocks]
(
	RowIndex int identity primary key,
	BlockNumber nvarchar(100),
	[Hash] nvarchar(100),
	ParentHash nvarchar(100),
	Nonce bigint,
	ExtraData nvarchar(max),
	Difficulty bigint,
	TotalDifficulty bigint,
	Size bigint,
	Miner nvarchar(100),
	GasLimit bigint,
	GasUsed bigint,
	[Timestamp] bigint,
	TransactionCount bigint
)

CREATE TABLE localhost.[Contracts]
(
	RowIndex int identity primary key,
	[Address] nvarchar(100),
	[Name] nvarchar(255),
	[ABI] nvarchar(max),
	[Code] nvarchar(max),
	Creator nvarchar(100),
	TransactionHash nvarchar(100)
)

CREATE TABLE localhost.TransactionLogs
(
	RowIndex int identity primary key,
	TransactionHash nvarchar(100),
	LogIndex bigint,
	[Address] nvarchar(100),
	Topics nvarchar(max),
	Topic0 nvarchar(100),
	[Data] nvarchar(max)
)

CREATE TABLE localhost.TransactionVmStacks 
(
	RowIndex int identity primary key,
	[Address] nvarchar(100),
	TransactionHash nvarchar(100),
	StructLogs nvarchar(max)
)