using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public class InMemoryEventQueue : IDecodedEventHandler
    {
        Queue<DecodedEvent> _queue = new Queue<DecodedEvent>();

        public Task<bool> HandleAsync(DecodedEvent decodedEvent)
        {
            _queue.Enqueue(decodedEvent);

            Debug.Write(JsonConvert.SerializeObject(decodedEvent));

            return Task.FromResult(true);
        }
    }

    public class QueueHandler : IDecodedEventHandler
    {
        public QueueHandler(ISubscriberQueue subscriberQueue)
        {
            SubscriberQueue = subscriberQueue;
        }

        public ISubscriberQueue SubscriberQueue { get; }

        public async Task<bool> HandleAsync(DecodedEvent decodedEvent)
        {
            await SubscriberQueue.AddMessage(decodedEvent);
            return true;
        }
    }
}
