using Moq;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.Handlers.Aggregators
{
    public abstract class EventAggregatorTestsBase
    {
        protected EventAggregatorConfiguration AggregatorConfig;
        protected EventSubscriptionStateDto EventSubscriptionState;
        protected EventAggregator Aggregator;
        protected Mock<IEventSubscription> MockEventSubscription;
        protected abstract EventAggregatorConfiguration CreateConfiguration();

        public EventAggregatorTestsBase()
        {
            MockEventSubscription = new Mock<IEventSubscription>();
            AggregatorConfig = CreateConfiguration();
            EventSubscriptionState = new EventSubscriptionStateDto();
            MockEventSubscription.Setup(s => s.State).Returns(EventSubscriptionState);
            Aggregator = new EventAggregator(MockEventSubscription.Object, 1, AggregatorConfig);
        }
    }
}

