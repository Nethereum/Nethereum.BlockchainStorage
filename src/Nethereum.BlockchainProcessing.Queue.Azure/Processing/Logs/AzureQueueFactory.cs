using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Queue.Azure.Processing.Logs
{
    public class AzureSubscriberQueueFactory : ISubscriberQueueFactory
    {
        public AzureSubscriberQueueFactory(string connectionString)
        {
            CloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudQueueClient = CloudStorageAccount.CreateCloudQueueClient();
        }

        public CloudStorageAccount CloudStorageAccount { get; }
        public CloudQueueClient CloudQueueClient { get; }

        public async Task<IQueue> GetSubscriberQueueAsync(ISubscriberQueueDto queueConfig)
        {
            if(queueConfig.Disabled) throw new Exception($"Subscriber Queue ({queueConfig.Id}) is disabled");

            return await GetOrCreateQueueAsync(queueConfig.Name).ConfigureAwait(false);
        }

        public async Task<IQueue> GetOrCreateQueueAsync(string queueName)
        {
            var queueReference = CloudQueueClient.GetQueueReference(queueName);

            await queueReference.CreateIfNotExistsAsync().ConfigureAwait(false);

            return new AzureQueue(queueReference);
        }
    }
}
