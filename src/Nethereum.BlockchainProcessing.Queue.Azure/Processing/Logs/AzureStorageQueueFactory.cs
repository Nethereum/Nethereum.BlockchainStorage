using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Queue.Azure.Processing.Logs
{
    public class AzureStorageQueueFactory // : ISubscriberQueueFactory
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
            var queueReference = CloudQueueClient.GetQueueReference(queueName);

            await queueReference.CreateIfNotExistsAsync().ConfigureAwait(false);

            return new AzureStorageQueue(queueReference);
        }

        public async Task ClearQueueAsync(string queueName)
        {
            var queueReference = CloudQueueClient.GetQueueReference(queueName);
            if(await queueReference.ExistsAsync())
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
