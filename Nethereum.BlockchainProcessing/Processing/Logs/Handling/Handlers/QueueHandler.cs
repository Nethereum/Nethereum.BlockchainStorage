using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public class QueueHandler : EventHandlerBase, IEventHandler
    {
        public QueueHandler(IEventSubscription subscription, long id, IQueue queue)
            :base(subscription, id)
        {
            Queue = queue;
        }

        public IQueue Queue { get; }

        public async Task<bool> HandleAsync(DecodedEvent decodedEvent)
        {
            await Queue.AddMessageAsync(decodedEvent);
            return true;
        }
    }
}
