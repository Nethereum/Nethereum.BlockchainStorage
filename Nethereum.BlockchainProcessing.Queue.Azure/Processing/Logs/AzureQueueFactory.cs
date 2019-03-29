using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Queue.Azure.Processing.Logs
{
    public class AzureSubscriberQueueFactory : ISubscriberQueueFactory
    {
        CloudQueueClient cloudQueueClient;

        public AzureSubscriberQueueFactory(string connectionString, ISubscriberQueueConfigurationFactory queueConfigurationFactory)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            QueueConfigurationFactory = queueConfigurationFactory;
        }

        public ISubscriberQueueConfigurationFactory QueueConfigurationFactory { get; }

        public async Task<ISubscriberQueue> GetSubscriberQueueAsync(long subscriberQueueId)
        {
            var queueConfig = await QueueConfigurationFactory.GetSubscriberQueueAsync(subscriberQueueId);
            if(queueConfig.Disabled) throw new Exception($"Subscriber Queue ({subscriberQueueId}) is disabled");

            var queueReference = cloudQueueClient.GetQueueReference(queueConfig.Name);

            await queueReference.CreateIfNotExistsAsync();

            return new AzureSubscriberQueue(queueReference);
        }
    }

    public class AzureSubscriberQueue : ISubscriberQueue
    {
        public AzureSubscriberQueue(CloudQueue cloudQueue)
        {
            CloudQueue = cloudQueue;
            Name = cloudQueue.Name;
        }

        public string Name {get; }

        public CloudQueue CloudQueue { get; }

        public async Task AddMessage(DecodedEvent decodedEvent)
        {
            await CloudQueue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(decodedEvent)));
        }
    }
}
