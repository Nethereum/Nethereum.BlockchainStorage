# Nethereum.BlockchainStore

A base library for repository style persistence of block chain data.

## StorageProcessorConsole

[Source](StorageProcessorConsole.cs)

This provides a quick-start for creating a processing console. It wraps up the processor scaffolding and provides a common layer for other storage adapters.

Feel free to look at the source code and use parts of it in isolation.  

### Processor Configuration
For dot net core all configuration is derived in the following order.  Settings lower down the chain will override earlier settings.  
This allows environmental variables and command line args to override base settings in configuration files.

The environment is set differently from other configuration as it is read first because other settings may be dependant on it. 
The environment "ASPNETCORE_ENVIRONMENT" is read by default from environmental variables and can be overriden by the command line.

* appsettings.json
* appsettings.{enviroment}.json
* user secrets (only when environment is "development")
* environmental variables
* command line args

### Configuration Settings
The following settings are applicable to all storage processing regardless of the repository layer.

For adapter specific configuration go to the relevant adapter project folder.

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
