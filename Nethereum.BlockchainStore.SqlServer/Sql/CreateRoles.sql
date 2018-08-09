use BlockchainStorage

CREATE ROLE localhost_users AUTHORIZATION dbo;
CREATE ROLE kovan_users AUTHORIZATION dbo;
CREATE ROLE main_users AUTHORIZATION dbo;
CREATE ROLE rinkeby_users AUTHORIZATION dbo;
CREATE ROLE ropsten_users AUTHORIZATION dbo;


GRANT ALTER ON SCHEMA :: localhost TO localhost_users;  
GRANT INSERT ON SCHEMA :: localhost TO localhost_users;  
GRANT SELECT ON SCHEMA :: localhost TO localhost_users;  
GRANT UPDATE ON SCHEMA :: localhost TO localhost_users;  
GRANT DELETE ON SCHEMA :: localhost TO localhost_users;  
GRANT EXECUTE ON SCHEMA :: localhost TO localhost_users;  

GRANT ALTER ON SCHEMA :: kovan TO kovan_users;  
GRANT INSERT ON SCHEMA :: kovan TO kovan_users;  
GRANT SELECT ON SCHEMA :: kovan TO kovan_users;  
GRANT UPDATE ON SCHEMA :: kovan TO kovan_users;  
GRANT DELETE ON SCHEMA :: kovan TO kovan_users;  
GRANT EXECUTE ON SCHEMA :: kovan TO kovan_users;  

GRANT ALTER ON SCHEMA :: main TO main_users;  
GRANT INSERT ON SCHEMA :: main TO main_users;  
GRANT SELECT ON SCHEMA :: main TO main_users;  
GRANT UPDATE ON SCHEMA :: main TO main_users;  
GRANT DELETE ON SCHEMA :: main TO main_users;  
GRANT EXECUTE ON SCHEMA :: main TO main_users;  

GRANT ALTER ON SCHEMA :: rinkeby TO rinkeby_users;  
GRANT INSERT ON SCHEMA :: rinkeby TO rinkeby_users;  
GRANT SELECT ON SCHEMA :: rinkeby TO rinkeby_users;  
GRANT UPDATE ON SCHEMA :: rinkeby TO rinkeby_users;  
GRANT DELETE ON SCHEMA :: rinkeby TO rinkeby_users;  
GRANT EXECUTE ON SCHEMA :: rinkeby TO rinkeby_users;  

GRANT ALTER ON SCHEMA :: ropsten TO ropsten_users;  
GRANT INSERT ON SCHEMA :: ropsten TO ropsten_users;  
GRANT SELECT ON SCHEMA :: ropsten TO ropsten_users;  
GRANT UPDATE ON SCHEMA :: ropsten TO ropsten_users;  
GRANT DELETE ON SCHEMA :: ropsten TO ropsten_users;  
GRANT EXECUTE ON SCHEMA :: ropsten TO ropsten_users;  

