using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public class EventRule : IDecodedEventHandler
    {
        public EventRule(EventSubscriptionStateDto state)
        {
            State = state;
        }

        public EventSubscriptionStateDto State { get; }

        public Task<bool> HandleAsync(DecodedEvent decodedEvent)
        {
            //TODO: Apply some config driven logic based on event or cumulative state
            return Task.FromResult(true);
        }
    }
}
