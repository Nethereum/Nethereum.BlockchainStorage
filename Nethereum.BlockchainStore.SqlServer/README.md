#  Blockchain Storage SQL Server (EF Core)

This SQL server implementation uses Entity Framework Core.

It allows the same database to be used for multiple blockchains / test nets.  

A different SQL Server schema is used for each block chain / test net (other wise block numbers etc would duplicate and data from different block chains would be merged).  
Each schema contains the same tables.

## Initial Setup

Use the instructions below to get started quickly with some default logins and permissions (which can be changed later).

A DB user is created for each schema and allocated permissions only to that schema.  This is to help prevent accidental overwriting of data.

* Prerequisites
	* Ensure you have sysadmin rights to a SQL server
* Setup Default Logins
	* Use SSMS (or some other query runner)
		* (Feel free to change the passwords in the script)
		* use master
		* Run Script : Sql\01 CreateDbLogins.sql
* Create the database and schema
	* From the command line 
		* cd to Nethereum.BlockchainStore.SqlServer 
		* run DbInitialCreate.bat
* Apply default DB roles and permissions
	* use BlockchainStorage - run 02 CreateAndApplyDbRoles.sql