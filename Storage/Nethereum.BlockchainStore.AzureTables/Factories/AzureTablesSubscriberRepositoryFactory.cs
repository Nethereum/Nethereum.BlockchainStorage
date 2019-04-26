using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using Nethereum.BlockchainStore.Repositories.Handlers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Factories
{
    public class AzureTablesSubscriberRepositoryFactory : ISubscriberStorageFactory
    {
        Dictionary<string, CloudTableSetup> _cloudTableSetups = new Dictionary<string, CloudTableSetup>();
        public AzureTablesSubscriberRepositoryFactory( 
            string azureStorageConnectionString,
            ISubscriberStorageRepository configurationFactory)
        {
            ConfigurationFactory = configurationFactory;
            AzureStorageConnectionString = azureStorageConnectionString;
        }

        public ISubscriberStorageRepository ConfigurationFactory { get; }
        public string AzureStorageConnectionString { get; }

        public Task<ILogHandler> GetLogRepositoryAsync(ISubscriberStorageDto config)
        {
           CloudTableSetup cloudTableSetup = GetCloudTableSetup(config.Name);
           var repo = cloudTableSetup.CreateTransactionLogRepository();
           var handler = new TransactionLogRepositoryHandler(repo);
           return Task.FromResult(handler as ILogHandler); 
        }

        private CloudTableSetup GetCloudTableSetup(string tablePrefix)
        {
            if(!_cloudTableSetups.TryGetValue(tablePrefix, out CloudTableSetup setup))
            {
                setup = new CloudTableSetup(AzureStorageConnectionString, tablePrefix);
                _cloudTableSetups.Add(tablePrefix, setup);
            }
            return setup;
        }

        public async Task DeleteTablesAsync()
        {
            foreach(var prefix in _cloudTableSetups.Keys)
            {
                await _cloudTableSetups[prefix].GetTransactionsLogTable().DeleteIfExistsAsync();
            }
        }
    }
}
