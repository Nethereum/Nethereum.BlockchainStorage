using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public class QueueHandler : IEventHandler
    {
        public long Id { get; }
        public QueueHandler(long id, ISubscriberQueue subscriberQueue)
        {
            Id = id;
            SubscriberQueue = subscriberQueue;
        }

        public ISubscriberQueue SubscriberQueue { get; }

        public async Task<bool> HandleAsync(DecodedEvent decodedEvent)
        {
            await SubscriberQueue.AddMessageAsync(decodedEvent);
            return true;
        }
    }
}
