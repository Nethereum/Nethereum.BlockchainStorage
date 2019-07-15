# Nethereum.BlockchainStore.EF.SqlServer

## Sql Server Setup Scripts

[Common Configuration](../)

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
