-- ========================================================================================
-- Create User as DBO template for Azure SQL Database and Azure SQL Data Warehouse Database
-- ========================================================================================
-- For login <login_name, sysname, login_name>, create a user in the database
CREATE USER localhost1
	FOR LOGIN localhost1
	WITH DEFAULT_SCHEMA = localhost
GO

-- Add user to the database owner role
EXEC sp_addrolemember N'db_owner', N'localhost1'
GO
