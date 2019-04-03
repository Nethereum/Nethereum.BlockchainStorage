using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.Handlers.Aggregators
{
    public abstract class EventAggregatorTestsBase
    {
        protected EventAggregatorConfiguration AggregatorConfig;
        protected EventSubscriptionStateDto EventSubscriptionState;
        protected EventAggregator Aggregator;

        protected abstract EventAggregatorConfiguration CreateConfiguration();

        public EventAggregatorTestsBase()
        {
            AggregatorConfig = CreateConfiguration();
            EventSubscriptionState = new EventSubscriptionStateDto();
            Aggregator = new EventAggregator(EventSubscriptionState, AggregatorConfig);
        }
    }
}

