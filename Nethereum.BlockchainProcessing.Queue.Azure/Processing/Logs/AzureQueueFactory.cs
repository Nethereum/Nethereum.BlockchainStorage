using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Queue.Azure.Processing.Logs
{
    public class AzureSubscriberQueueFactory : ISubscriberQueueFactory
    {
        public AzureSubscriberQueueFactory(string connectionString, ISubscriberQueueConfigurationFactory queueConfigurationFactory)
        {
            CloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudQueueClient = CloudStorageAccount.CreateCloudQueueClient();
            QueueConfigurationFactory = queueConfigurationFactory;
        }

        public ISubscriberQueueConfigurationFactory QueueConfigurationFactory { get; }
        public CloudStorageAccount CloudStorageAccount { get; set; }
        public CloudQueueClient CloudQueueClient { get; set; }

        public async Task<ISubscriberQueue> GetSubscriberQueueAsync(long subscriberQueueId)
        {
            var queueConfig = await QueueConfigurationFactory.GetSubscriberQueueAsync(subscriberQueueId);
            if(queueConfig.Disabled) throw new Exception($"Subscriber Queue ({subscriberQueueId}) is disabled");

            var queueReference = CloudQueueClient.GetQueueReference(queueConfig.Name);

            await queueReference.CreateIfNotExistsAsync();

            return new AzureSubscriberQueue(queueReference);
        }
    }
}
