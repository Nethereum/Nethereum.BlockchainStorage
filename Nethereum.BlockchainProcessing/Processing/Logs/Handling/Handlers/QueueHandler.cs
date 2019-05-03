using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers
{

    public class QueueHandler : EventHandlerBase, IEventHandler
    {
        public QueueHandler(IEventSubscription subscription, long id, IQueue queue, IQueueMessageMapper mapper = null)
            :base(subscription, id)
        {

            Queue = queue;
            Mapper = mapper ?? QueueMessageMapper.Default;
        }

        public IQueue Queue { get; }
        public IQueueMessageMapper Mapper { get; }

        public async Task<bool> HandleAsync(DecodedEvent decodedEvent)
        {
            var msgToQueue = Mapper.Map(decodedEvent);
            await Queue.AddMessageAsync(msgToQueue);
            return true;
        }
    }

}
