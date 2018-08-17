USE [master]
GO
/*
IMPORTANT - PLEASE READ

CREATES preset logins for the SQL Server
*/

-- Sysadmin User for migrations
--	the user name and password are referenced in a connection string within the appsettings.json file in the project folder
--	you can change the password or use an alternative login - but remember to change the connection string before attempting a migration or db update
IF NOT EXISTS(select sid from sys.syslogins where name = 'blockchain_storage_admin') 
BEGIN
	CREATE LOGIN blockchain_storage_admin WITH PASSWORD = 'eV5YMcv5aoDOsyEtJeCf'
END
GO

EXEC master..sp_addsrvrolemember @loginame = N'blockchain_storage_admin', @rolename = N'sysadmin'
GO

-- Default logins for each schema - feel free to change passwords
IF NOT EXISTS(select sid from sys.syslogins where name = 'blockchaindbo1') 
BEGIN
	CREATE LOGIN blockchaindbo1 WITH PASSWORD = 'bALLfMA1wBlJCzSGZhkO';
END
GO
IF NOT EXISTS(select sid from sys.syslogins where name = 'localhost1') 
BEGIN
	CREATE LOGIN localhost1 WITH PASSWORD = 'MeLLfMA1wBlJCzSGZhkO';
END
GO
IF NOT EXISTS(select sid from sys.syslogins where name = 'ropsten1') 
BEGIN
	CREATE LOGIN ropsten1 WITH PASSWORD = 'MgOumZZ1LVbV5gSfen3M';
END
GO
IF NOT EXISTS(select sid from sys.syslogins where name = 'kovan1') 
BEGIN
	CREATE LOGIN kovan1 WITH PASSWORD = 'm4753ct9kYMospex';
END
GO
IF NOT EXISTS(select sid from sys.syslogins where name = 'rinkeby1') 
BEGIN
	CREATE LOGIN rinkeby1 WITH PASSWORD = 'rzNk9PyskZg0jLIl';
END
GO
IF NOT EXISTS(select sid from sys.syslogins where name = 'main1') 
BEGIN
	CREATE LOGIN main1 WITH PASSWORD = 'ix2tjLkUEmPJ2Ah6';
END
GO
