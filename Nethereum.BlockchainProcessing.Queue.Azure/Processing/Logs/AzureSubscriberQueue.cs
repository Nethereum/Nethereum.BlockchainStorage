using Microsoft.WindowsAzure.Storage.Queue;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Queue.Azure.Processing.Logs
{
    public class AzureSubscriberQueue : ISubscriberQueue
    {
        public AzureSubscriberQueue(CloudQueue cloudQueue)
        {
            CloudQueue = cloudQueue;
            Name = cloudQueue.Name;
        }

        public string Name {get; }

        public CloudQueue CloudQueue { get; }

        public async Task AddMessageAsync(object content)
        {
            await CloudQueue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(content)));
        }
    }
}
