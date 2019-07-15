rem packing code generators
del /S *.*.nupkg
del /S *.*.snupkg

cd Nethereum.BlockchainStore.Search
CALL :restorepack
cd ..

cd Nethereum.BlockchainStore.AzureTables
CALL :restorepack
cd ..

cd Nethereum.BlockchainStore.CosmosCore
CALL :restorepack
cd ..

cd Nethereum.BlockchainStore.Csv
CALL :restorepack
cd ..

cd Nethereum.BlockchainStore.EF.Sqlite
CALL :restorepack
cd ..

cd Nethereum.BlockchainStore.EF.SqlServer
CALL :restorepack
cd ..

cd Nethereum.BlockchainStore.EFCore.Sqlite
CALL :restorepack
cd ..

cd Nethereum.BlockchainStore.EFCore.SqlServer
CALL :restorepack
cd ..

cd Nethereum.BlockchainStore
CALL :restorepack
cd ..

cd Nethereum.BlockchainStore.Entities
CALL :restorepack
cd ..

cd Nethereum.BlockchainStore.Entities.Mapping
CALL :restorepack
cd ..

cd Nethereum.BlockchainStore.EF
CALL :restorepack
cd ..

cd Nethereum.BlockchainStore.EFCore
CALL :restorepack
cd ..

cd Nethereum.BlockchainStore.MongoDb
CALL :restorepack
cd ..

setlocal
set DIR=%~dp0
set OUTPUTDIR=%~dp0\deploy-packages

for /R %DIR% %%a in (*.nupkg) do xcopy "%%a" "%OUTPUTDIR%"
for /R %DIR% %%a in (*.snupkg) do xcopy "%%a" "%OUTPUTDIR%"

EXIT /B %ERRORLEVEL%

:restorepack
dotnet restore -c Release
dotnet pack -c Release --include-symbols -p:SymbolPackageFormat=snupkg
EXIT /B 0