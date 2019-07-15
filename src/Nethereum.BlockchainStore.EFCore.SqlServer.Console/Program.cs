using Nethereum.BlockchainStore.Console;
using System.Threading;

namespace Nethereum.BlockchainStore.EFCore.SqlServer.Console
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var storageProcessorConsole = new StorageProcessorConsole<BlockchainStoreRepositoryFactory>(
                args,
                "Nethereum.BlockchainStorage.EFCore.SqlServer",
                config => new BlockchainStoreRepositoryFactory(SqlServerCoreBlockchainDbContextFactory.Create(config))
                );

            return storageProcessorConsole.ExecuteAsync(
                new CancellationToken())
                .Result;
        }
    }
}
