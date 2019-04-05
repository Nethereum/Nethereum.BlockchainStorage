namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public abstract class EventHandlerBase
    {
        public long Id { get; }
        public EventSubscriptionStateDto State { get; }

        protected EventHandlerBase(long id, EventSubscriptionStateDto state)
        {
            Id = id;
            State = state;
        }
    }
}
