dotnet ef migrations add -c BlockchainDbContext_localhost InitialCreate
dotnet ef migrations add -c BlockchainDbContext_ropsten InitialCreate
dotnet ef migrations add -c BlockchainDbContext_rinkeby InitialCreate
dotnet ef migrations add -c BlockchainDbContext_kovan InitialCreate
dotnet ef migrations add -c BlockchainDbContext_main InitialCreate
dotnet ef database update -c BlockchainDBContext_localhost
dotnet ef database update -c BlockchainDBContext_ropsten
dotnet ef database update -c BlockchainDBContext_rinkeby
dotnet ef database update -c BlockchainDBContext_kovan
dotnet ef database update -c BlockchainDBContext_main