using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Queue.Azure
{
    public class AzureStorageQueue<TSource, TQueueMessage> : AzureStorageQueue where TSource : class where TQueueMessage : class
    {
        public AzureStorageQueue(CloudQueue cloudQueue, Func<TSource, TQueueMessage> mapper) : base(cloudQueue)
        {
            Mapper = mapper;
        }

        public Func<TSource, TQueueMessage> Mapper { get; }

        public override Task AddMessageAsync(object content)
        {
            if(content is TSource src)
            {
                var msg = Mapper(src);
                return base.AddMessageAsync(msg);
            }

            return Task.CompletedTask;
        }
    }

    public class AzureStorageQueue : IQueue
    {
        public AzureStorageQueue(CloudQueue cloudQueue)
        {
            CloudQueue = cloudQueue;
        }

        public string Name => CloudQueue.Name;

        public CloudQueue CloudQueue { get; }
        public async Task<int> GetApproxMessageCountAsync()
        {
            await CloudQueue.FetchAttributesAsync().ConfigureAwait(false);
            var count = CloudQueue.ApproximateMessageCount;
            return count ?? 0;
        } 

        public virtual async Task AddMessageAsync(object content)
        {
            var contentAsJson = JsonConvert.SerializeObject(content);
            var message = new CloudQueueMessage(contentAsJson);
            await CloudQueue.AddMessageAsync(message).ConfigureAwait(false);
        }
    }
}
