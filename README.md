# Nethereum Blockchain Store

The Nethereum blockhain store is a library that allows the retrieving and storage of the Ethereum Blockchain by connecting a Node using RPC.

The current implementation processes and stores Blocks, Transactions, Logs, Contracts and the VM Stack.

The VM is specific for Geth, so if connecting to another implementation the VM will need to be disabled.

Storage is implemented in Azure table storage, allowing for a simple (and cheap) way to store the data. 
The Azure table storage can be easily replaced with other implementations like Azure Sql if needed. All the repositories are abstracted through interfaces providing easy replacement.

## Entities
![Entities](Entities.png)

## Sample of Console processor

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

### Transaction Processors
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

A top level transaction processor orchastrates the acess to the granular implementations:

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

## Execution
Finally after configuration we can execute the processing blocks as follows

```csharp
  while (startBlock <= endBlock)
  {
    await _procesor.ProcessBlockAsync(startBlock).ConfigureAwait(false);
    startBlock = startBlock + 1;           
  }
```