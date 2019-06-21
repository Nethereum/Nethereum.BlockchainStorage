# Nethereum Blockchain Store

## Blockchain Storage

The storage folder contains a base library and a set of adapters the allow retrieving and storage of the Ethereum Blockchain by connecting a Node using RPC.

The current implementation processes and stores Blocks, Transactions, Logs, Contracts and the VM Stack.

The VM Stack retrieval/parsing is specific for Geth, so if connecting to another implementation the VM will need to be disabled.

All the repositories are abstracted through interfaces providing easy replacement and extensions.

Implementations are written for the following:

All are .Net Standard 2.0 libraries for flexibility.

- Azure Table Storage
- Cosmos Db
- CSV
- Sqlite (Entity Framework Core)
- Sqlite (Entity Framework)
- Sql Server (Entity Framework Core)
- Sql Server (Entity Framework)
- Mongo Db

## Entities
![Entities](Entities.png)

## Samples

Simple console apps have been created to demonstrate storage using different adapters.

* [Azure Table Storage](Nethereum.BlockchainStore.AzureTables.Core.Console/Program.cs)
* [Cosmos](Nethereum.BlockchainStore.CosmosCore.Console/Program.cs)
* [CSV](Nethereum.BlockchainStore.Csv.Console/Program.cs)
* [Sql Server - Entity Framework Core](Nethereum.BlockchainStore.EFCore.SqlServer.Console/Program.cs)
* [Sqlite - Entity Framework Core](Nethereum.BlockchainStore.EFCore.Sqlite.Console/Program.cs)
* [Mongo Db](Nethereum.BlockchainStore.MongoDb.Console/Program.cs)


## Transaction Processors
Ethereum can be divided on three types of transactions, contract creation, value transfer and contract specific transaction (which can also include a value transfer)

Each one of these types has different ways needs for identification, processing and storage, so there abstracted as and initialised as follows:

```csharp
var contractTransactionProcessor = new ContractTransactionProcessor(_web3, contractRepository,
                transactionRepository, addressTransactionRepository, vmStackRepository, logRepository);
var contractCreationTransactionProcessor = new ContractCreationTransactionProcessor(_web3, contractRepository,
                transactionRepository, addressTransactionRepository);
var valueTrasactionProcessor = new ValueTransactionProcessor(transactionRepository,
                addressTransactionRepository);
```

The ContractCreationTransactionProcessor has the option to Enabled or Disable the VM processing. Even if using Geth VM processing is rather slow.

A top level transaction processor orchestrates the access to the granular implementations:

```csharp
  var transactionProcessor = new TransactionProcessor(_web3, contractTransactionProcessor,
                valueTrasactionProcessor, contractCreationTransactionProcessor);
```

The transaction processor has the also the option to enable or disable each one the specific transaction processors.

### Blockchain processor

Finally the blockchain processor is configured with the blockchain repository and transaction processor.
In this example we disable the vm processing beforehand.

```csharp
    transactionProcessor.ContractTransactionProcessor.EnabledVmProcessing = false;
    _procesor = new BlockProcessor(_web3, blockRepository, transactionProcessor);
```

This can be complemented for performance with the Blockchain post processor. This preconfigured implementation only processes the VM and the contract transactions. 

```csharp
 _procesor = new BlockVmPostProcessor(_web3, blockRepository, transactionProcessor);
 ```




