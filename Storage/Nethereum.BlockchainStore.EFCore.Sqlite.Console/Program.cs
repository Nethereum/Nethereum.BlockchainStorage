using Microsoft.Extensions.Logging;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainStore.Processing;
using Nethereum.Configuration;
using Nethereum.Logging;

namespace Nethereum.BlockchainStore.EFCore.Sqlite.Console
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var log = ApplicationLogging.CreateConsoleLogger<Program>().ToILog();

            var appConfig = ConfigurationUtils
                .Build(args, userSecretsId: "Nethereum.BlockchainStorage.EFCore.Sqlite");

            var blockchainSourceConfiguration = BlockchainSourceConfigurationFactory.Get(appConfig);
            var contextFactory = new SqliteBlockchainDbContextFactory(appConfig.GetBlockchainStorageConnectionString());
            var repositoryFactory = new BlockchainStoreRepositoryFactory(contextFactory);

            return StorageProcessorConsole.Execute(repositoryFactory, blockchainSourceConfiguration, log: log).Result;
        }
    }
}
