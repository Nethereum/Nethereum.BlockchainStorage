rem packing code generators
del /S *.*.nupkg
del /S *.*.snupkg

cd Nethereum.BlockchainProcessing
CALL :restorepack
cd ..

setlocal
set DIR=%~dp0
set OUTPUTDIR=%~dp0\packages

for /R %DIR% %%a in (*.nupkg) do xcopy "%%a" "%OUTPUTDIR%"
for /R %DIR% %%a in (*.snupkg) do xcopy "%%a" "%OUTPUTDIR%"

EXIT /B %ERRORLEVEL%

:restorepack
dotnet restore -c Release
dotnet pack -c Release --include-symbols -p:SymbolPackageFormat=snupkg
EXIT /B 0