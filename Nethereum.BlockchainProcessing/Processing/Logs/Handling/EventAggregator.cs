using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public class EventAggregator : IDecodedEventHandler
    {
        public const string STATE_KEY_AGGREGATOR_INVOCATIONS = "AggregatorInvocations";

        public EventAggregator(EventSubscriptionStateDto state)
        {
            State = state;
        }

        public int InvocationCount => State.GetInt(STATE_KEY_AGGREGATOR_INVOCATIONS);
        public EventSubscriptionStateDto State { get; }

        public Task<bool> HandleAsync(DecodedEvent decodedEvent)
        {
            State.Increment(STATE_KEY_AGGREGATOR_INVOCATIONS);
            decodedEvent.Aggregates[STATE_KEY_AGGREGATOR_INVOCATIONS] = InvocationCount;
            return Task.FromResult(true);
        }
    }
}
