# Nethereum.LogProcessing.Samples

Here are some quick start samples to demonstrate processing the chain.
They are written as unit tests against known data on a publicly available testnet.
This means you can run them and expect the same results.

## Log Processing (aka event log)
These are "log driven" or "log first" examples. 
This is considerably faster than crawling each block and transaction and event.
It is still possible to retrieve the block and transaction related to the relevant logs.
* [Simple Event Log Processing](SimpleEventLogProcessing.cs) START HERE!
* [Event Log Enumeration](EventLogEnumeration.cs)
* [Writing Events To A Queue](WritingEventsToAQueue.cs)
* [Writing Events To Azure Storage](WritingEventsToAzureStorage.cs)

