<<<<<<< HEAD
# Nethereum.BlockchainProcessing

## Summary

**Read data from the chain sequentially and do what you need with it**

Why??!!!

Loads of reason!  Here are a few:

* Store chain data locally for analysis, auditing or easy lookup
* Trigger custom functionality based on chain activity
* Respond to chain events to drive specific functionality
* Populate a search index based on chain data

This processing library enables you to plug in your own functionality whilst it deals with the chain data navigation and conversion.

Built on core Nethereum libraries:
* Proven Ethereum integration
* Easy data decoding
* Easy data mapping
* On-going updates and support 

**Traversing Chain Data**

There are two main options to traverse the chain.  

* Read block by block (which reads in all transactions and events). 
* Read events in block order (quicker if the event data is all you need).

**Filters**

Create simple or complex filters to isolate and process the data you want with minimal setup.

**Architecture**

In additition to the processors there are a library of tools and components that can be used separately.

**Related projects**

The following projects build on the processing libary to provide pluggable off the shelf and re-usable components.
* Blockchain storage, store chain data - includes adapters for Azure Table Storage, CSV, SQL Server, Sqlite, Cosmos.  https://github.com/Nethereum/Nethereum.BlockchainStorage/
* Azure Search - index chain data easily  https://github.com/Nethereum/Nethereum.BlockchainStorage/tree/master/Nethereum.BlockchainStore.Search.Samples

**WHERE DO I START?**

Start with the samples!!  https://github.com/Nethereum/Nethereum.BlockchainProcessing/tree/master/Nethereum.BlockchainProcessing.Samples


# Component Level Information

## Processors
Processors are orchestrators. They navigate and retrieve or receive block chain data, filter it, invoke custom handlers and invoke lower level processors.
In general the processors do not need customisation and you don't need to write your own. They are there to walk the chain and pass the relevant data to *your* handlers.

Handlers are pluggable interfaces.  To inject your own behaviour - you should create a class that implements the relevant handler interface and inject it.

## PROCESSING COMPONENTS
Here is a high level processing overview. 

* Block Chain Processing
	* Sequentially enumerating the block chain using the injected processing strategy
* Processing Strategy 
	* start and end block configuration
	* custom error handling and retry logic
	* minimum block confirmation configuration
	* invokes injected Block Processor with the current block number
* Block Processor
	* retrieves block data and it's transactions from the chain 
	* applies filters
	* invokes injected block handler
	* invokes Transaction Processor
* Transaction Processor
	* applies filters
	* retrieves transaction logs
	* invokes specific transaction processor (calling contract, contract creation, value transaction)
		* lower level transaction processors invoke transaction handlers
	* invokes transaction log processor

## Classes

### BlockchainProcessor
The BlockchainProcessor reads blocks sequentially.
It enumerates the blocks and passes each to the processing strategy.

* It can run continuously or for a preset block number range.
* Has a facility to pick up where it left off (e.g. following a restart).
* Can wait for a number of block confirmations before reading from the chain.
* Has in built fault tolerance algorithms.

### Pluggable Handlers
* Pluggable Handlers are passed the relevant blocks, transactions and logs.
* Processors only invoke handlers if there are either no filters, or at least one filter condition is met.
* The Handler interfaces are simple and allow implementations to do anything they need to.
For example they may store relevant data in a database, or populate a queue or call a custom web service.
* A "HandlerContainer" class is used to pass handlers into the block processor factory.
* Not all handler types need an implementation, the HandlerContainer has default implementations. 

### Filters
* Filters can restrict which transactions and logs are sent to the handlers. 
* The filter execution takes place in the processing layer which prevents handlers from being invoked unecessarily.

### BlockchainProcessingStrategy
The blockchain processor uses the strategy to:
* Get the last block processed
* Get the current max block number from chain
* Pause between retries
* Wait for the next block to be added to the chain
* Invoke the injected block processor

### BlockProcessor
* This retrieves and processes an individual block and passes it to a block handler.
* Applies a block filter to avoid calling a handler or transaction processor unecessarily.
* Passes blocks to a TransactionProcessor.

### TransactionProcessor
* Receives all transactions from a block
* Retrieves receipts for transactions from the chain
* Applies filters (transaction, receipt, transaction and receipt)
* Identifies transaction category:
	* calling a contract
	* creating a contract
	* value transaction
* Passes to transaction handlers.

### TransactionRouter
* This implements the ITransactionHandler interface.
* It's purpose is to take a transaction and route it (conditionally) to one or many transaction handlers.
* It allows specific handlers to be invoked when the transaction meets their criteria.
For example, this means handlers can be added to handle calls to specific functions on a contract.
For a token contract, one handler may handle calls to "transfer" whilst another may handle a call to "approve".
* The conditions are dynamic and flexible and can be created based on any of the transaction information or any external calls that may be necessary.

### TransactionLogRouter
* This implements the ITransactionLogHandler interface.
* It's purpose is to take a transaction log and route it (conditionally) to one or many handlers.
* It allows specific handlers to be invoked when the log meets their criteria.
For example, this means handlers can be added to handle specific events on a contract.
For a token contract, one handler may handle "Transfer" whilst another may handle "Approval".
* The conditions are dynamic and flexible and can be created based on any of the transaction log information or any external calls that may be necessary.
=======
# Nethereum Blockchain Store

## Blockchain Storage

The Nethereum blockhain store is a library that allows the retrieving and storage of the Ethereum Blockchain by connecting a Node using RPC.

The current implementation processes and stores Blocks, Transactions, Logs, Contracts and the VM Stack.

The VM Stack retrieval/parsing is specific for Geth, so if connecting to another implementation the VM will need to be disabled.

All the repositories are abstracted through interfaces providing easy replacement.

Implementations are written for the following:

All are .Net Standard 2.0 libraries for flexibility.

- Azure Table Storage
- Cosmos Db
- CSV
- Sqlite (Entity Framework Core)
- Sqlite (Entity Framework)
- Sql Server (Entity Framework Core)
- Sql Server (Entity Framework)

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
## Processor Configuration
For dot net core all configuration is derived in the following order.  Settings lower down the chain will override earlier settings.  
This allows environmental variables and command line args to override base settings in configuration files.

The environment is set differently from other configuration as it is read first because other settings may be dependant on it. 
The environment "ASPNETCORE_ENVIRONMENT" is read by default from environmental variables and can be overriden by the command line.

* appsettings.json
* appsettings.{enviroment}.json
* user secrets (only when environment is "development")
* environmental variables
* command line args

### Storage Processor Configuration Settings
The following settings are applicable to all storage processing regardless of the repository layer.

When provided in the command line, ensure the argument is prefixed with "--".  The dashes should be ommitted when using other configuration sources (appsettings etc).

* --ASPNETCORE_ENVIRONMENT
  - Defines the environment the application is running in.
  - User defined (e.g. "development", "CI", "staging").
  - Default is blank.
  - Use "development" if you require values from visual studio user secrets.
* --Blockchain
  - The name of the blockchain/client e.g. localhost, rinkeby, main.
  - Default is localhost.
* --MinimumBlockNumber
  - A number dictating the lowest block number to process - instead of starting at 0.
  - Useful when "FromBlock" has not neen specified.
  - Default is 0.
* --MinimumBlockConfirmations
  - The number of blocks to remain behind the front of the chain
  - Default is 0.
* --FromBlock
  - The starting block number.
  - Default is 0.
* --ToBlock
  - The end block number, ommit to continue processing indefinitely (storing new blocks as they appear).
* --BlockchainUrl 
  - The url of the ethereum client - e.g. http://localhost:8545.  
  - If using Infura this URL should include the access key e.g. "https://rinkeby.infura.io/v3/{access_key}")
* --PostVm 
  - true/false.
  - indicates if post VM processing is required.
  - defaults to false.
* --ProcessBlockTransactionsInParallel
  - true/false
  - Enables each transaction within a block in parallel.
  - Defaults to true.

### Azure Tables Configuration Settings

User Secrets Id: Nethereum.BlockchainStore.AzureTables

* --AzureStorageConnectionString  (the full azure connection string used for Azure storage - found in the Azure portal)

### Cosmos Repository Configuration Settings

User Secrets Id: Nethereum.BlockchainStore.CosmosCore.UserSecrets

* --CosmosEndpointUri
* --CosmosAccessKey
* --CosmosDbTag 
  - A tag appended to the default database name in Cosmos
  - Allows the database name to differ between environments or block chain targets.
  - Default database name is BlockchainStorage.
  - Adding "Rinkeby" as a CosmosDbTag results in a database name - BlockchainStorageRinkeby.

### Sqlite Entity Framework Core Configuration Settings

The database connection name can be dynamic based on the DbSchema setting.

Default Db Connection Name = "BlockchainDbStorage".
When the DbSchema setting is not empty - the schema name is appended to the default connection string name.

``` json
{
  "ConnectionStrings" : {
    "BlockchainDbStorage" : "Data Source=C:/temp/Blockchain_EFCore.db",
    "BlockchainDbStorage_localhost" : "Data Source=C:/temp/Blockchain_EFCore_localhost.db",
    "BlockchainDbStorage_ropsten" : "Data Source=C:/temp/Blockchain_EFCore_ropsten.db",
    "BlockchainDbStorage_rinkeby" : "Data Source=C:/temp/Blockchain_EFCore_rinkeby.db" 
  }
}
```
### CSV Repository Configuration Settings

* --CsvOutputPath  (The directory where the CSV files will be written)

#### CSV Specific Info

All of the repository storage implementations perform Upsert operations - they will update an existing record if it already exists else insert a new one.
This allows the storage processor to be re-run for the same block range.  This may be necessary if the a process crashed whilst writing transactions or if there has been a fork in the chain.
For CSV a true upsert is not really pragmatic or performant.  However it is still desirable to avoid writing duplicate records to a file.
If the CSV file already exists, the repo will read all records in the file and store a hash for each in memory.  
Before writing new records, the hash is compared to the in-memory store and if the hash is found the record is presumed to be a duplicate and is not written.

### Sql Server Setup Scripts

This applies to dot net core and full framework.  The Db schema is the same.

Setup scripts are available in the "Nethereum.BlockchainStore.EFCore.SqlServer" project within the Sql folder.
A DB user is created for each schema and allocated permissions only to that schema.  This is to help prevent accidental overwriting of data.

* Prerequisites
	* Ensure you have sysadmin rights to a SQL server
* Setup Default Logins
	* Use SSMS (or some other query runner)
		* (Feel free to change the passwords in the script - they are only there for a quick start in a development only environment)
		* use master
		* Run Script : Sql\01 CreateDbLogins.sql
* Create the database yourself - the default DB name is BlockchainStorage.
* Apply default DB roles and permissions script against your database
	* 02 CreateAndApplyDbRoles.sql (ensure it is run against the correct database)
* Apply the scripts relating to the schemas you require (e.g. SqlServerBlockchainDbContext_dbo.sql, SqlServerBlockchainDbContext_rinkeby.sql etc)

### Sql Server and Sqlite (Full Framework) Configuration

For an example - check out the App.config in the "Nethereum.BlockchainStore.EF.Tests" project. 
(Not relevant to dot net core!) 
>>>>>>> Nethereum.BlockchainStorage/master

