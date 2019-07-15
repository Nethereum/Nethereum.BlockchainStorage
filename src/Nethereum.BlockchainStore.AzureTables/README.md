# Nethereum.BlockchainStore.AzureTables

## Azure Tables Configuration Settings

[Common Configuration](../)

User Secrets Id: Nethereum.BlockchainStore.AzureTables

* --AzureStorageConnectionString  (the full azure connection string used for Azure storage - found in the Azure portal)

## Azure Table Storage In More Detail

The console processor sample demonstrates how to process a range of blocks and initialisation of the library with the azure repository.

### Initialisation of the different components

#### Table setup 

To simplify the setup of the Azure tables a generic bootstrapper allows to create the connection and validates the existance of the different tables.
Tables have been assigned generic names (Transactions, Blocks, Contracts, TransactionsLog, AdressTransactions, TransactionsVmStack), each one of them can be prefixed with an environment i.e. "Morden" to allow multiple blockchains in an storage account.

```csharp
var tableSetup = new CloudTableSetup(connectionString);

_contractTable = tableSetup.GetContractsTable(prefix);
var transactionsTable = tableSetup.GetTransactionsTable(prefix);
var addressTransactionsTable = tableSetup.GetAddressTransactionsTable(prefix);
var blocksTable = tableSetup.GetBlocksTable(prefix);
var logTable = tableSetup.GetTransactionsLogTable(prefix);
var vmStackTable = tableSetup.GetTransactionsVmStackTable(prefix);
```

### Repository setup
The repositories provides the wrapper access to the azure tables.

```csharp
var blockRepository = new BlockRepository(blocksTable);
var transactionRepository = new TransactionRepository(transactionsTable);
var addressTransactionRepository = new AddressTransactionRepository(addressTransactionsTable);
var contractRepository = new ContractRepository(_contractTable);
var logRepository = new TransactionLogRepository(logTable);
var vmStackRepository = new TransactionVMStackRepository(vmStackTable);
```
