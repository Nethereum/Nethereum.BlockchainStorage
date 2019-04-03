using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public class SearchIndexHandler : IEventHandler
    {
        public SearchIndexHandler(ISubscriberSearchIndex subscriberSearchIndex)
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
