# Nethereum.BlockchainStore.CosmosCore

## Cosmos Repository Configuration Settings

[Common Configuration](../)

User Secrets Id: Nethereum.BlockchainStore.CosmosCore.UserSecrets

* --CosmosEndpointUri
* --CosmosAccessKey
* --CosmosDbTag 
  - A tag appended to the default database name in Cosmos
  - Allows the database name to differ between environments or block chain targets.
  - Default database name is BlockchainStorage.
  - Adding "Rinkeby" as a CosmosDbTag results in a database name - BlockchainStorageRinkeby.
