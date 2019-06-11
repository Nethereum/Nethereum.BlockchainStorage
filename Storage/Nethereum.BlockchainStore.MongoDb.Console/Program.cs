using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainStore.MongoDb.Bootstrap;
using Nethereum.BlockchainStore.Processing;
using Nethereum.Configuration;

namespace Nethereum.BlockchainStore.MongoDb.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var appConfig = ConfigurationUtils
                .Build(args, userSecretsId: "Nethereum.BlockchainStore.MongoDb.UserSecrets")
                .AddConsoleLogging();

            var configuration = BlockchainSourceConfigurationFactory.Get(appConfig);
            var repositoryFactory = MongoDbRepositoryFactory.Create(appConfig);
            return StorageProcessorConsole.Execute(repositoryFactory, configuration).Result;
        }
    }
}
