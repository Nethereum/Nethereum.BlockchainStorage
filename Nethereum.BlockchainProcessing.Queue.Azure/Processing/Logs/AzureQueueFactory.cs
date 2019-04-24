using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Queue.Azure.Processing.Logs
{
    public class AzureSubscriberQueueFactory : ISubscriberQueueFactory
    {
        public AzureSubscriberQueueFactory(string connectionString, ISubscriberQueueConfigurationRepository queueConfigurationFactory = null)
        {
            CloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudQueueClient = CloudStorageAccount.CreateCloudQueueClient();
            QueueConfigurationFactory = queueConfigurationFactory;
        }

        public ISubscriberQueueConfigurationRepository QueueConfigurationFactory { get; }
        public CloudStorageAccount CloudStorageAccount { get; }
        public CloudQueueClient CloudQueueClient { get; }

        public async Task<IQueue> GetSubscriberQueueAsync(long subscriberId, long subscriberQueueId)
        {
            var queueConfig = await QueueConfigurationFactory.GetSubscriberQueueAsync(subscriberId, subscriberQueueId);
            if(queueConfig.Disabled) throw new Exception($"Subscriber Queue ({subscriberQueueId}) is disabled");

            return await GetOrCreateQueueAsync(queueConfig.Name);
        }

        public async Task<IQueue> GetOrCreateQueueAsync(string queueName)
        {
            var queueReference = CloudQueueClient.GetQueueReference(queueName);

            await queueReference.CreateIfNotExistsAsync();

            return new AzureQueue(queueReference);
        }
    }
}
