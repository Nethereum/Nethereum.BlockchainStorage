namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public abstract class EventHandlerBase
    {
        public IEventSubscription Subscription { get; }
        public long Id { get; }

        protected EventHandlerBase(IEventSubscription subscription, long id)
        {
            Subscription = subscription;
            Id = id;
        }
    }
}
