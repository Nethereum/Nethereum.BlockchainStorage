# Nethereum.BlockchainProcessing

## Summary

**Read data from the chain sequentially and do what you need with it**

Why??!!!

Loads of reasons!  Here are a few:

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

* LogProcessing: Read event logs in block order (quicker if the log data is all you need).
* BlockProcessing: Read block by block (which reads in all transactions and events). 

**Filters**

Create simple or complex filters to isolate and process the data you want with minimal setup.

**Architecture**

In addition to the processors there are a library of tools and components that can be used separately.

**Storage and Search**

The following projects build on the processing library to provide pluggable off the shelf and re-usable components.
* Blockchain storage, store chain data - includes adapters for Azure Table Storage, CSV, SQL Server, Sqlite, Cosmos.  [Storage Samples](Storage/)
* Azure Search - index chain data easily  [Search Samples](Storage/Nethereum.BlockchainStore.Search.Samples)

**WHERE DO I START?**

Start with the samples!!  

[Log Processing](Nethereum.LogProcessing.Samples)

[Block Processing](Nethereum.BlockProcessing.Samples)

