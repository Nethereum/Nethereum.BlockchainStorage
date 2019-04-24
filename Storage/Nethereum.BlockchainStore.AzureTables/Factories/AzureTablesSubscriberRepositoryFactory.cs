using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using Nethereum.BlockchainStore.Repositories.Handlers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Factories
{
    public class AzureTablesSubscriberRepositoryFactory : ISubscriberRepositoryFactory
    {
        Dictionary<string, CloudTableSetup> _cloudTableSetups = new Dictionary<string, CloudTableSetup>();
        public AzureTablesSubscriberRepositoryFactory( 
            string azureStorageConnectionString,
            ISubscriberRepositoryConfigurationRepository configurationFactory)
        {
            ConfigurationFactory = configurationFactory;
            AzureStorageConnectionString = azureStorageConnectionString;
        }

        public ISubscriberRepositoryConfigurationRepository ConfigurationFactory { get; }
        public string AzureStorageConnectionString { get; }

        public async Task<ILogHandler> GetLogRepositoryAsync(long subscriberReposistoryId)
        {
           var config = await ConfigurationFactory.GetSubscriberRepositoryAsync(subscriberReposistoryId);
           CloudTableSetup cloudTableSetup = GetCloudTableSetup(config.Name);
           var repo = cloudTableSetup.CreateTransactionLogRepository();
           var handler = new TransactionLogRepositoryHandler(repo);
           return handler; 
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
