using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainStore.AzureTables.Bootstrap.EventProcessingConfiguration;
using Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Samples.SAS.Configuration
{
    public class AzureTablesConfigurationRepoTests
    {
        [Fact]
        public async Task UpsertAndRetrieveSubscribers()
        {
            var config = TestConfiguration.LoadConfig();
            string azureStorageConnectionString = config["AzureStorageConnectionString"];
            
            var cloudTableSetup = new EventProcessingCloudTableSetup(azureStorageConnectionString, "test");
            var subscriberRepo = cloudTableSetup.GetSubscriberRepository();

            try     
            {

                var configRepo = new AzureEventProcessingConfigurationRepository(subscriberRepo);

                var subscriber1 = new SubscriberDto
                {
                    Id = 1,
                    PartitionId = 1,
                    Disabled = false,
                    Name = "One"
                };

                var subscriber2 = new SubscriberDto
                {
                    Id = 2,
                    PartitionId = 1,
                    Disabled = false,
                    Name = "Two"
                };

                var subscriber3 = new SubscriberDto
                {
                    Id = 3,
                    PartitionId = 2,
                    Disabled = false,
                    Name = "One"
                };


                await subscriberRepo.UpsertAsync(subscriber1);
                await subscriberRepo.UpsertAsync(subscriber2);
                await subscriberRepo.UpsertAsync(subscriber3);

                var partition1Subscribers = await configRepo.GetSubscribersAsync(partitionId: 1);
                var partition2Subscribers = await configRepo.GetSubscribersAsync(partitionId: 2);

                Assert.Equal(2, partition1Subscribers.Length);
                Assert.Single(partition2Subscribers);
            }
            finally
            {
                await cloudTableSetup.GetSubscribersTable().DeleteIfExistsAsync();
            }
        }
    }
}
