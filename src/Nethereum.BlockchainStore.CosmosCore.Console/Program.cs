using Nethereum.BlockchainStore.Console;
using Nethereum.BlockchainStore.CosmosCore.Bootstrap;
using System.Threading;

namespace Nethereum.BlockchainStore.CosmosCore.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var storageProcessorConsole = new StorageProcessorConsole<CosmosRepositoryFactory>(
                args,
                "Nethereum.BlockchainStore.CosmosCore.UserSecrets",
                config => CosmosRepositoryFactory.Create(config)
                );

            return storageProcessorConsole.ExecuteAsync(
                new CancellationToken())
                .Result;
        }
    }
}
