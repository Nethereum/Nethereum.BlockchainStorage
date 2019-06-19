
# BlockProcessing Component Level Information

## Processors

Processors are orchestrators. They navigate and retrieve or receive blockchain data, filter it, invoke custom handlers and invoke lower level processors.
In general the processors do not need customisation and you don't need to write your own. They are there to walk the chain and pass the relevant data to *your* handlers.

Handlers are pluggable interfaces.  To inject your own behaviour - you should create a class that implements the relevant handler interface and inject it.

## PROCESSING COMPONENTS
Here is a high level processing overview. 

* Blockchain Processing
	* Sequentially enumerating the block chain using the injected processing strategy
* Processing Strategy 
	* start and end block configuration
	* custom error handling and retry logic
	* minimum block confirmation configuration
	* invokes injected Block Processor with the current block number
* Block Processor
	* retrieves block data and its transactions from the chain 
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
* Its purpose is to take a transaction and route it (conditionally) to one or many transaction handlers.
* It allows specific handlers to be invoked when the transaction meets their criteria.
For example, this means handlers can be added to handle calls to specific functions on a contract.
For a token contract, one handler may handle calls to "transfer" whilst another may handle a call to "approve".
* The conditions are dynamic and flexible and can be created based on any of the transaction information or any external calls that may be necessary.

### TransactionLogRouter
* This implements the ITransactionLogHandler interface.
* Its purpose is to take a transaction log and route it (conditionally) to one or many handlers.
* It allows specific handlers to be invoked when the log meets their criteria.
For example, this means handlers can be added to handle specific events on a contract.
For a token contract, one handler may handle "Transfer" whilst another may handle "Approval".
* The conditions are dynamic and flexible and can be created based on any of the transaction log information or any external calls that may be necessary.

