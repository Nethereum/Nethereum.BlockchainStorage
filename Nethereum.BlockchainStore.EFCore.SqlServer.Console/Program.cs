using Nethereum.BlockchainStore.Processing;

namespace Nethereum.BlockchainStore.EFCore.SqlServer.Console
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var appConfig = ConfigurationUtils.Build(args, userSecretsId: "Nethereum.BlockchainStorage.EFCore.SqlServer");
            var blockchainConfig = BlockchainSourceConfigurationPresets.Get(appConfig);
            var dbContextFactory = SqlServerCoreBlockchainDbContextFactory.Create(appConfig);
            var repositoryFactory = new BlockchainStoreRepositoryFactory(dbContextFactory);
            return ProcessorConsole.Execute(repositoryFactory, blockchainConfig).Result;
        }
    }
}
