# Nethereum.BlockchainProcessing.InMemory.Console

A sample to demonstrate reading sequentially from the blockchain and configuring handlers to invoke custom behaviour.

The example handlers write to the console each time they are invoked.  They demonstrate how simple it is to read from the blockchain.  

Writing your own handlers is just a matter of creating your own class that implements the relevant handler interface.

## Filters

You can use a pre defined filter and create your own.  For instance you may only be interested in transactions sent to or from a specific address. 

When filters are provided, the handlers are only invoked if one or more of the filters match.

You could ignore filters and implement the criteria in the handlers.  However, using filters will minimise unecessary processing - such as retrieving a transaction receipt for a transaction you are not interested in.

[Go To The Code](Program.cs)