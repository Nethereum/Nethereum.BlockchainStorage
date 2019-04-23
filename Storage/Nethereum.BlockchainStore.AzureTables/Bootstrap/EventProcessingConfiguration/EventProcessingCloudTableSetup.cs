using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration;

namespace Nethereum.BlockchainStore.AzureTables.Bootstrap.EventProcessingConfiguration
{
    public class EventProcessingCloudTableSetup : CloudTableSetupBase
    {
        public EventProcessingCloudTableSetup(string connectionString, string prefix) : base(connectionString, prefix)
        {
        }

        public CloudTable GetSubscribersTable() => GetPrefixedTable("Subscribers");

        public ISubscriberRepository GetSubscriberRepository() => new SubscriberRepository(GetSubscribersTable());
    }
}
