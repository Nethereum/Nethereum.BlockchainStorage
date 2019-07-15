using Microsoft.Extensions.Logging;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainStore.Processing;
using Microsoft.Configuration.Utils;
using Microsoft.Logging.Utils;

namespace Nethereum.BlockchainStore.EFCore.SqlServer.Console
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var log = ApplicationLogging.CreateConsoleLogger<Program>().ToILog();

            var appConfig = ConfigurationUtils
                .Build(args, userSecretsId: "Nethereum.BlockchainStorage.EFCore.SqlServer");

            var blockchainConfig = BlockchainSourceConfigurationFactory.Get(appConfig);
            var dbContextFactory = SqlServerCoreBlockchainDbContextFactory.Create(appConfig);
            var repositoryFactory = new BlockchainStoreRepositoryFactory(dbContextFactory);
            return StorageProcessorConsole.Execute(repositoryFactory, blockchainConfig, log: log).Result;
        }
    }
}
