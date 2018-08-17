dotnet ef migrations remove -f -c SqlServerBlockchainDbContext_dbo
dotnet ef migrations remove -f -c SqlServerBlockchainDbContext_localhost
dotnet ef migrations remove -f -c SqlServerBlockchainDbContext_ropsten
dotnet ef migrations remove -f -c SqlServerBlockchainDbContext_rinkeby
dotnet ef migrations remove -f -c SqlServerBlockchainDbContext_kovan
dotnet ef migrations remove -f -c SqlServerBlockchainDbContext_main
dotnet ef migrations add -c SqlServerBlockchainDbContext_dbo InitialCreate
dotnet ef migrations add -c SqlServerBlockchainDbContext_localhost InitialCreate
dotnet ef migrations add -c SqlServerBlockchainDbContext_ropsten InitialCreate
dotnet ef migrations add -c SqlServerBlockchainDbContext_rinkeby InitialCreate
dotnet ef migrations add -c SqlServerBlockchainDbContext_kovan InitialCreate
dotnet ef migrations add -c SqlServerBlockchainDbContext_main InitialCreate
dotnet ef database update -c SqlServerBlockchainDbContext_dbo
dotnet ef database update -c SqlServerBlockchainDbContext_localhost
dotnet ef database update -c SqlServerBlockchainDbContext_ropsten
dotnet ef database update -c SqlServerBlockchainDbContext_rinkeby
dotnet ef database update -c SqlServerBlockchainDbContext_kovan
dotnet ef database update -c SqlServerBlockchainDbContext_main