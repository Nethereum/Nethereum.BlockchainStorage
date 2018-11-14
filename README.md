# Nethereum.BlockchainProcessing
Blockchain core processing library

A library of tools to help read from the block chain.

## BlockchainProcessor
The BlockchainProcessor reads blocks sequentially.
It enumerates the blocks and passes each to the processing strategy.

* It can run continuously or for a preset block number range.
* Has a facility to pick up where it left off (e.g. following a restart).
* Can wait for a number of block confirmations before reading from the chain.
* Has in built fault tolerance algorithms.

### Handlers
* Pluggable Handlers are passed the relevant blocks, transactions and logs.
* The Handler interfaces are simple and allow implementations to do anything they need to.
For example they may store relevant data in a database, or populate a queue or call a custom web service.

### Filters
* Filters can restrict which transactions and logs are sent to the handlers. 

## BlockchainProcessingStrategy
The blockchain processor uses the strategy to:
* Get the last block processed
* Get the current max block number from chain
* Process a blocks
* Pause between retries
* Wait for the next block to be added to the chain

## BlockProcessor
* This retrieves and processes an individual block and passes it to a block handler.
* Applies a block filter to avoid calling a handler or transaction processor unecessarily.
* Passes blocks to a TransactionProcessor.

## TransactionProcessor
* Receives all transactions from a block
* Retrieves receipts for transactions
* Applies filters (transaction, receipt, transaction and receipt)
* Identifies transaction category:
	* calling a contract
	* creating a contract
	* value transaction
* Passes to transaction handlers.

## TransactionRouter
This implements the ITransactionHandler interface.
It's purpose is to take a transaction and route it (conditionally) to one or many transaction handlers.
It allows specific handlers to be invoked when the transaction meets their criteria.
For example, this means handlers can be added to handle calls to specific functions on a contract.
For a token contract, one handler may handle calls to "transfer" whilst another may handle a call to "approve".
The conditions are flexible and can be created based on any of the transaction information.
