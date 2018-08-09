-- ========================================================================================
-- Create User as DBO template for Azure SQL Database and Azure SQL Data Warehouse Database
-- ========================================================================================
-- For login <login_name, sysname, login_name>, create a user in the database

use BlockchainStorage
GO

CREATE USER localhost1
	FOR LOGIN localhost1
	WITH DEFAULT_SCHEMA = localhost
GO

-- Add user to the database owner role
EXEC sp_addrolemember N'localhost_users', N'localhost1'

GO

CREATE USER ropsten1
	FOR LOGIN ropsten1
	WITH DEFAULT_SCHEMA = ropsten
GO

-- Add user to the database owner role
EXEC sp_addrolemember N'ropsten_users', N'ropsten1'
GO

CREATE USER kovan1
	FOR LOGIN kovan1
	WITH DEFAULT_SCHEMA = kovan
GO

-- Add user to the database owner role
EXEC sp_addrolemember N'kovan_users', N'kovan1'
GO

CREATE USER rinkeby1
	FOR LOGIN rinkeby1
	WITH DEFAULT_SCHEMA = rinkeby
GO

-- Add user to the database owner role
EXEC sp_addrolemember N'rinkeby_users', N'rinkeby1'
GO

CREATE USER main1
	FOR LOGIN main1
	WITH DEFAULT_SCHEMA = main
GO

-- Add user to the database owner role
EXEC sp_addrolemember N'main_users', N'main1'
GO
