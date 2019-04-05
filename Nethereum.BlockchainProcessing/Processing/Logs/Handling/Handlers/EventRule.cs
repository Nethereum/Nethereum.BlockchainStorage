using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public class EventRule : IEventHandler
    {
        public long Id { get; }

        public EventRule(long id, EventSubscriptionStateDto state)
        {
            Id = id;
            State = state;
        }

        public EventSubscriptionStateDto State { get; }

        public Task<bool> HandleAsync(DecodedEvent decodedEvent)
        {
            //TODO: Apply some config driven logic based on event or cumulative state
            //option to return false to prevent further handlers from being invoked
            throw new System.NotImplementedException("EventRule is yet to be implemented");
        }
    }
}
