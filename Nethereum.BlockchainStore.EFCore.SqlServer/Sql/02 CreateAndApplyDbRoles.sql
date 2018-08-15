/*
IMPORTANT - PLEASE READ

Ensure you have applied the EF migrations to the database first!

This script adds the logins for the sql server as users of the database
It adds groups with permissions to the different schemas
It adds the users to the relevant groups

*/

USE [BlockchainStorage]
GO
IF NOT EXISTS(SELECT uid FROM sysusers WHERE name = 'ropsten1')
BEGIN
	CREATE USER [ropsten1] FOR LOGIN [ropsten1] WITH DEFAULT_SCHEMA=[ropsten]
END
GO

IF NOT EXISTS(SELECT uid FROM sysusers WHERE name = 'rinkeby1')
BEGIN
	CREATE USER [rinkeby1] FOR LOGIN [rinkeby1] WITH DEFAULT_SCHEMA=[rinkeby]
END
GO

IF NOT EXISTS(SELECT uid FROM sysusers WHERE name = 'main1')
BEGIN
	CREATE USER [main1] FOR LOGIN [main1] WITH DEFAULT_SCHEMA=[main]
END
GO

IF NOT EXISTS(SELECT uid FROM sysusers WHERE name = 'localhost1')
BEGIN
	CREATE USER [localhost1] FOR LOGIN [localhost1] WITH DEFAULT_SCHEMA=[localhost]
END
GO

IF NOT EXISTS(SELECT uid FROM sysusers WHERE name = 'kovan1')
BEGIN
	CREATE USER [kovan1] FOR LOGIN [kovan1] WITH DEFAULT_SCHEMA=[kovan]
END
GO

IF NOT EXISTS(SELECT uid from sysusers where issqlrole = 1 and name = 'ropsten_users')
BEGIN
	CREATE ROLE [ropsten_users]
END
GO

IF NOT EXISTS(SELECT uid from sysusers where issqlrole = 1 and name = 'rinkeby_users')
BEGIN
	CREATE ROLE [rinkeby_users]
END
GO

IF NOT EXISTS(SELECT uid from sysusers where issqlrole = 1 and name = 'main_users')
BEGIN
	CREATE ROLE [main_users]
END
GO

IF NOT EXISTS(SELECT uid from sysusers where issqlrole = 1 and name = 'localhost_users')
BEGIN
	CREATE ROLE [localhost_users]
END
GO

IF NOT EXISTS(SELECT uid from sysusers where issqlrole = 1 and name = 'kovan_users')
BEGIN
	CREATE ROLE [kovan_users]
END
GO


IF NOT EXISTS(select 1 from sys.database_role_members where user_name(member_principal_id) = 'ropsten1' and user_name(role_principal_id) = 'ropsten_users')
BEGIN
	ALTER ROLE [ropsten_users] ADD MEMBER [ropsten1]
END
GO

IF NOT EXISTS(select 1 from sys.database_role_members where user_name(member_principal_id) = 'rinkeby1' and user_name(role_principal_id) = 'rinkeby_users')
BEGIN
	ALTER ROLE [rinkeby_users] ADD MEMBER [rinkeby1]
END
GO

IF NOT EXISTS(select 1 from sys.database_role_members where user_name(member_principal_id) = 'main1' and user_name(role_principal_id) = 'main_users')
BEGIN
	ALTER ROLE [main_users] ADD MEMBER [main1]
END
GO

IF NOT EXISTS(select 1 from sys.database_role_members where user_name(member_principal_id) = 'localhost1' and user_name(role_principal_id) = 'localhost_users')
BEGIN
	ALTER ROLE [localhost_users] ADD MEMBER [localhost1]
END
GO

IF NOT EXISTS(select 1 from sys.database_role_members where user_name(member_principal_id) = 'kovan1' and user_name(role_principal_id) = 'kovan_users')
BEGIN
	ALTER ROLE [kovan_users] ADD MEMBER [kovan1]
END
GO


GRANT ALTER ON SCHEMA :: localhost TO localhost_users;  
GO
GRANT INSERT ON SCHEMA :: localhost TO localhost_users;  
GO
GRANT SELECT ON SCHEMA :: localhost TO localhost_users;  
GO
GRANT UPDATE ON SCHEMA :: localhost TO localhost_users;  
GO
GRANT DELETE ON SCHEMA :: localhost TO localhost_users;  
GO
GRANT EXECUTE ON SCHEMA :: localhost TO localhost_users;  
GO

GRANT ALTER ON SCHEMA :: kovan TO kovan_users;  
GO
GRANT INSERT ON SCHEMA :: kovan TO kovan_users;  
GO
GRANT SELECT ON SCHEMA :: kovan TO kovan_users;  
GO
GRANT UPDATE ON SCHEMA :: kovan TO kovan_users;  
GO
GRANT DELETE ON SCHEMA :: kovan TO kovan_users;  
GO
GRANT EXECUTE ON SCHEMA :: kovan TO kovan_users;  
GO

GRANT ALTER ON SCHEMA :: main TO main_users;  
GO
GRANT INSERT ON SCHEMA :: main TO main_users;  
GO
GRANT SELECT ON SCHEMA :: main TO main_users;  
GO
GRANT UPDATE ON SCHEMA :: main TO main_users;  
GO
GRANT DELETE ON SCHEMA :: main TO main_users;  
GO
GRANT EXECUTE ON SCHEMA :: main TO main_users;  
GO

GRANT ALTER ON SCHEMA :: rinkeby TO rinkeby_users;  
GO
GRANT INSERT ON SCHEMA :: rinkeby TO rinkeby_users;  
GO
GRANT SELECT ON SCHEMA :: rinkeby TO rinkeby_users;  
GO
GRANT UPDATE ON SCHEMA :: rinkeby TO rinkeby_users;  
GO
GRANT DELETE ON SCHEMA :: rinkeby TO rinkeby_users;  
GO
GRANT EXECUTE ON SCHEMA :: rinkeby TO rinkeby_users;  
GO

GRANT ALTER ON SCHEMA :: ropsten TO ropsten_users;  
GO
GRANT INSERT ON SCHEMA :: ropsten TO ropsten_users;  
GO
GRANT SELECT ON SCHEMA :: ropsten TO ropsten_users;  
GO
GRANT UPDATE ON SCHEMA :: ropsten TO ropsten_users;  
GO
GRANT DELETE ON SCHEMA :: ropsten TO ropsten_users;  
GO
GRANT EXECUTE ON SCHEMA :: ropsten TO ropsten_users;  
GO