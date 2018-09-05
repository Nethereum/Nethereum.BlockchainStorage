using Nethereum.BlockchainStore.CosmosCore.Bootstrap;
using Nethereum.BlockchainStore.Processing;

namespace Nethereum.BlockchainStore.CosmosCore.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var configuration = ProcessorConfigurationPresets.Get(args);
            var repositoryFactory = CosmosRepositoryFactory.Create();
            repositoryFactory.EnsureDatabaseAndCollections().Wait();
            return ProcessorConsole.Execute(repositoryFactory, configuration).Result;
        }
    }
}
