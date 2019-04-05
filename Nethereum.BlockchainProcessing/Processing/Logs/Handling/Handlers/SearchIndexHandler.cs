using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public class SearchIndexHandler : EventHandlerBase, IEventHandler
    {
        public SearchIndexHandler(long id, EventSubscriptionStateDto state, ISubscriberSearchIndex subscriberSearchIndex)
            :base(id, state)
        {
            SubscriberSearchIndex = subscriberSearchIndex;
        }

        public ISubscriberSearchIndex SubscriberSearchIndex { get; }

        public async Task<bool> HandleAsync(DecodedEvent decodedEvent)
        {
            await SubscriberSearchIndex.Index(decodedEvent);
            return true;
        }
    }
}
