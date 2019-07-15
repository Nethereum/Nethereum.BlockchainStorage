# Nethereum.BlockchainStore.EF.Sqlite

## Sqlite Entity Framework Configuration Settings

[Common Configuration](../)

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