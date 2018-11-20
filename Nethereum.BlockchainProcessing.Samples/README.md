# Nethereum.BlockchainProcessing.Samples

Here are some quick start samples to demonstrate processing the chain.
They are written as unit tests against known data on a publicly available testnet.
This means you can run them and expect the same results.

## Block and Transaction Processing (includes Event Logs)
If you need transactions and logs then use these examples.

* [Block And Transaction Enumeration](BlockAndTransactionEnumeration.cs)
* [Filtering Transactions](FilterTransactions.cs)
* [Listening For A Specific Event](ListeningForASpecificEvent.cs)
* [Listening For A Specific Function Call](ListeningForASpecificFunctionCall.cs)
* [Conditional Transaction Routing](ConditionalTransactionRouting.cs)
* [Conditional Transaction Log Routing](ConditionalTransactionLogRouting.cs)

## Event Log Processing (excludes transactions)
If you are ONLY interested in event logs - then this is a much faster way of enumerating the chain and listening for events.
* [Event Log Enumeration](EventLogEnumeration.cs)