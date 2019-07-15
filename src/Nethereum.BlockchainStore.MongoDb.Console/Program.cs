using Nethereum.BlockchainStore.Console;
using Nethereum.BlockchainStore.MongoDb.Bootstrap;
using System.Threading;

namespace Nethereum.BlockchainStore.MongoDb.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var storageProcessorConsole = new StorageProcessorConsole<MongoDbRepositoryFactory>(
                args,
                "Nethereum.BlockchainStore.MongoDb.UserSecrets",
                config => MongoDbRepositoryFactory.Create(config)
                );

            return storageProcessorConsole.ExecuteAsync(
                new CancellationToken())
                .Result;
        }
    }
}
