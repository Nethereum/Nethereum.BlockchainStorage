using Nethereum.BlockchainStore.Processing;
using Nethereum.BlockchainStore.Processors.Transactions;

namespace Nethereum.BlockchainStore.EFCore.Sqlite.Console
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var appConfig = ConfigurationUtils.Build(args, userSecretsId: "Nethereum.BlockchainStorage.EFCore.Sqlite");
            var blockchainSourceConfiguration = BlockchainSourceConfigurationPresets.Get(appConfig);
            var contextFactory = new SqliteBlockchainDbContextFactory(appConfig.GetBlockchainStorageConnectionString());
            var repositoryFactory = new BlockchainStoreRepositoryFactory(contextFactory);

            var filterContainer = new FilterContainer(
                TransactionFilter.ValueGreaterThanZero(),
                TransactionReceiptFilter.OnlyNewContracts());

            blockchainSourceConfiguration.PostVm = true;

            return ProcessorConsole.Execute(repositoryFactory, blockchainSourceConfiguration, filterContainer ,useGeth: true).Result;
        }
    }
}
