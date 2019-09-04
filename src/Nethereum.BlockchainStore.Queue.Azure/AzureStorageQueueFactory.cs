using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Queue.Azure
{
    public class AzureStorageQueueFactory : IQueueFactory
    {
        public AzureStorageQueueFactory(string connectionString)
        {
            CloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudQueueClient = CloudStorageAccount.CreateCloudQueueClient();
        }

        public CloudStorageAccount CloudStorageAccount { get; }
        public CloudQueueClient CloudQueueClient { get; }

        public async Task<IQueue> GetOrCreateQueueAsync(string queueName)
        {
            var queueReference = await GetOrCreateQueueReference(queueName: queueName, retryNumber: 0).ConfigureAwait(false);
            return new AzureStorageQueue(queueReference);
        }

        public async Task<IQueue> GetOrCreateQueueAsync<TSource, TQueueMessage>(string queueName, Func<TSource, TQueueMessage> mapper)
            where TSource : class where TQueueMessage : class
        {
            var queueReference = await GetOrCreateQueueReference(queueName: queueName, retryNumber: 0).ConfigureAwait(false);
            return new AzureStorageQueue<TSource, TQueueMessage>(queueReference, mapper);
        }

        private async Task<CloudQueue> GetOrCreateQueueReference(string queueName, int retryNumber)
        {
            try
            {
                var queueReference = CloudQueueClient.GetQueueReference(queueName);
                await queueReference.CreateIfNotExistsAsync().ConfigureAwait(false);
                return queueReference;
            }
            catch (StorageException ex) when (ex.Message.StartsWith("The specified queue is being deleted"))
            {
                if (retryNumber > 5) throw;

                retryNumber++;
                await Task.Delay(2000 * retryNumber);
                return await GetOrCreateQueueReference(queueName: queueName, retryNumber: retryNumber).ConfigureAwait(false);
            }
        }

        public async Task ClearQueueAsync(string queueName)
        {
            var queueReference = CloudQueueClient.GetQueueReference(queueName);
            if (await queueReference.ExistsAsync())
            {
                await queueReference.ClearAsync().ConfigureAwait(false);
            }
        }

        public async Task DeleteQueueAsync(string queueName)
        {
            var queueReference = CloudQueueClient.GetQueueReference(queueName);
            await queueReference.DeleteIfExistsAsync().ConfigureAwait(false);
        }
    }
}
