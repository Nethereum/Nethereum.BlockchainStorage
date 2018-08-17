dotnet ef migrations script --context BlockchainDbContext_dbo --output ./Sql/BlockchainDbContext_dbo.sql
dotnet ef migrations script --context BlockchainDbContext_localhost --output ./Sql/BlockchainDbContext_localhost.sql
dotnet ef migrations script --context BlockchainDbContext_kovan --output ./Sql/BlockchainDbContext_kovan.sql
dotnet ef migrations script --context BlockchainDbContext_rinkeby --output ./Sql/BlockchainDbContext_rinkeby.sql
dotnet ef migrations script --context BlockchainDbContext_ropsten --output ./Sql/BlockchainDbContext_ropsten.sql
dotnet ef migrations script --context BlockchainDbContext_main --output ./Sql/BlockchainDbContext_main.sql