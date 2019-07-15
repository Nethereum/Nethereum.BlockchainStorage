using Nethereum.BlockchainStore.Console;
using System.Threading;

namespace Nethereum.BlockchainStore.Csv.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var storageProcessorConsole = new StorageProcessorConsole<CsvBlockchainStoreRepositoryFactory>(
                args, 
                "Nethereum.BlockchainStore.Csv.UserSecrets",
                config => new CsvBlockchainStoreRepositoryFactory(config["CsvOutputPath"]),
                repoFactory => repoFactory.DisposeRepositories()
                );

            return storageProcessorConsole.ExecuteAsync( 
                new CancellationToken())
                .Result;
        }
    }
}
