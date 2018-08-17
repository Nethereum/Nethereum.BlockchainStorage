dotnet ef migrations remove -f -c BlockchainDbContext_dbo
dotnet ef migrations remove -f -c BlockchainDbContext_localhost
dotnet ef migrations remove -f -c BlockchainDbContext_ropsten
dotnet ef migrations remove -f -c BlockchainDbContext_rinkeby
dotnet ef migrations remove -f -c BlockchainDbContext_kovan
dotnet ef migrations remove -f -c BlockchainDbContext_main
dotnet ef migrations add -c BlockchainDbContext_dbo InitialCreate
dotnet ef migrations add -c BlockchainDbContext_localhost InitialCreate
dotnet ef migrations add -c BlockchainDbContext_ropsten InitialCreate
dotnet ef migrations add -c BlockchainDbContext_rinkeby InitialCreate
dotnet ef migrations add -c BlockchainDbContext_kovan InitialCreate
dotnet ef migrations add -c BlockchainDbContext_main InitialCreate
dotnet ef database update -c BlockchainDBContext_dbo
dotnet ef database update -c BlockchainDBContext_localhost
dotnet ef database update -c BlockchainDBContext_ropsten
dotnet ef database update -c BlockchainDBContext_rinkeby
dotnet ef database update -c BlockchainDBContext_kovan
dotnet ef database update -c BlockchainDBContext_main