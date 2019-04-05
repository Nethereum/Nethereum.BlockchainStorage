using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public class SearchIndexHandler : IEventHandler
    {
        public long Id { get; }
        public SearchIndexHandler(long id, ISubscriberSearchIndex subscriberSearchIndex)
        {
            Id = id;
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
