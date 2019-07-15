using Nethereum.BlockchainStore.Console;
using System.Threading;

namespace Nethereum.BlockchainStore.EFCore.Sqlite.Console
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var storageProcessorConsole = new StorageProcessorConsole<BlockchainStoreRepositoryFactory>(
                args,
                "Nethereum.BlockchainStorage.EFCore.Sqlite",
                config => new BlockchainStoreRepositoryFactory(new SqliteBlockchainDbContextFactory(config.GetBlockchainStorageConnectionString()))
                );

            return storageProcessorConsole.ExecuteAsync(
                new CancellationToken())
                .Result;
        }
    }
}
