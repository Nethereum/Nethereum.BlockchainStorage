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

        public Task<IQueue> GetOrCreateQueueAsync(string queueName) => GetOrCreateQueueAsync(queueName: queueName, retryNumber: 0);

        private async Task<IQueue> GetOrCreateQueueAsync(string queueName, int retryNumber)
        {
            try 
            { 
                var queueReference = CloudQueueClient.GetQueueReference(queueName);

                await queueReference.CreateIfNotExistsAsync().ConfigureAwait(false);

                return new AzureStorageQueue(queueReference);
            }
            catch (StorageException ex) when (ex.Message.StartsWith("The specified queue is being deleted"))
            {
                if(retryNumber > 5) throw;

                retryNumber++;
                await Task.Delay(2000 * retryNumber);
                return await GetOrCreateQueueAsync(queueName: queueName, retryNumber: retryNumber).ConfigureAwait(false);
            }
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
