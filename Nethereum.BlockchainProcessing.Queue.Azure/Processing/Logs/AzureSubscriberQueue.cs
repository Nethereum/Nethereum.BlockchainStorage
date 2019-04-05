using Microsoft.WindowsAzure.Storage.Queue;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Queue.Azure.Processing.Logs
{
    public class AzureQueue : IQueue
    {
        public AzureQueue(CloudQueue cloudQueue)
        {
            CloudQueue = cloudQueue;
        }

        public string Name => CloudQueue.Name;

        public CloudQueue CloudQueue { get; }

        public async Task AddMessageAsync(object content)
        {
            var contentAsJson = JsonConvert.SerializeObject(content);
            var message = new CloudQueueMessage(contentAsJson);
            await CloudQueue.AddMessageAsync(message);
        }
    }
}
