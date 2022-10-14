dotnet ef migrations script --context SqlServerBlockchainDbContext_dbo --output ./Sql/SqlServerBlockchainDbContext_dbo.sql
dotnet ef migrations script --context SqlServerBlockchainDbContext_localhost --output ./Sql/SqlServerBlockchainDbContext_localhost.sql
dotnet ef migrations script --context SqlServerBlockchainDbContext_kovan --output ./Sql/SqlServerBlockchainDbContext_kovan.sql
dotnet ef migrations script --context SqlServerBlockchainDbContext_rinkeby --output ./Sql/SqlServerBlockchainDbContext_rinkeby.sql
dotnet ef migrations script --context SqlServerBlockchainDbContext_ropsten --output ./Sql/SqlServerBlockchainDbContext_ropsten.sql
dotnet ef migrations script --context SqlServerBlockchainDbContext_main --output ./Sql/SqlServerBlockchainDbContext_main.sql
dotnet ef migrations script --context SqlServerBlockchainDbContext_goerli --output ./Sql/SqlServerBlockchainDbContext_goerli.sql
dotnet ef migrations script --context SqlServerBlockchainDbContext_sepolia --output ./Sql/SqlServerBlockchainDbContext_sepolia.sql