using Nethereum.BlockchainProcessing.BlockchainProxy;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{

    public class EventHandlerFactory: IEventHandlerFactory
    {
        public EventHandlerFactory(
            IBlockchainProxyService blockchainProxy, 
            IEventProcessingConfigurationDb configDb, 
            ISubscriberQueueFactory subscriberQueueFactory,
            ISubscriberSearchIndexFactory subscriberSearchIndexFactory)
            :this(
                 configDb, 
                 configDb, 
                 blockchainProxy,  
                 configDb, 
                 blockchainProxy, 
                 subscriberQueueFactory,
                 subscriberSearchIndexFactory,
                 configDb)
        {
        }

        public EventHandlerFactory(
            IEventSubscriptionStateFactory stateFactory, 
            IEventContractQueryConfigurationFactory contractQueryFactory,
            IContractQuery contractQueryHandler,
            IEventAggregatorConfigurationFactory eventAggregatorConfigurationFactory,
            IGetTransactionByHash getTransactionProxy,
            ISubscriberQueueFactory subscriberQueueFactory,
            ISubscriberSearchIndexFactory subscriberSearchIndexFactory,
            IEventRuleConfigurationFactory eventRuleConfigurationFactory)
        {
            StateFactory = stateFactory;
            ContractQueryFactory = contractQueryFactory;
            ContractQueryHandler = contractQueryHandler;
            EventAggregatorConfigurationFactory = eventAggregatorConfigurationFactory;
            GetTransactionProxy = getTransactionProxy;
            SubscriberQueueFactory = subscriberQueueFactory;
            SubscriberSearchIndexFactory = subscriberSearchIndexFactory;
            EventRuleConfigurationFactory = eventRuleConfigurationFactory;
        }

        public IEventSubscriptionStateFactory StateFactory { get; }
        public IEventContractQueryConfigurationFactory ContractQueryFactory { get; }
        public IContractQuery ContractQueryHandler { get; }
        public IEventAggregatorConfigurationFactory EventAggregatorConfigurationFactory { get; }
        public IGetTransactionByHash GetTransactionProxy { get; }
        public ISubscriberQueueFactory SubscriberQueueFactory { get; }
        public ISubscriberSearchIndexFactory SubscriberSearchIndexFactory { get; }
        public IEventRuleConfigurationFactory EventRuleConfigurationFactory { get; }

        public async Task<IEventHandler> LoadAsync(IEventSubscription subscription, EventHandlerDto config)
        { 
            switch (config.HandlerType)
            {
                case EventHandlerType.Rule:
                    var ruleConfig = await EventRuleConfigurationFactory.GetEventRuleConfigurationAsync(config.Id);
                    return new EventRule(subscription, config.Id, ruleConfig);
                case EventHandlerType.Aggregate:
                    var aggregatorConfig = await EventAggregatorConfigurationFactory.GetEventAggregationConfigurationAsync(config.Id);
                    return new EventAggregator(subscription, config.Id, aggregatorConfig);
                case EventHandlerType.ContractQuery:
                    var queryConfig = await ContractQueryFactory.GetContractQueryConfigurationAsync(config.Id);
                    return new ContractQueryEventHandler(subscription, config.Id, ContractQueryHandler, queryConfig);
                case EventHandlerType.Queue:
                    var queue = await SubscriberQueueFactory.GetSubscriberQueueAsync(config.SubscriberQueueId);
                    return new QueueHandler(subscription, config.Id, queue);
                case EventHandlerType.GetTransaction:
                    return new GetTransactionEventHandler(subscription, config.Id, GetTransactionProxy);
                case EventHandlerType.Index:
                    var searchIndex = await SubscriberSearchIndexFactory.GetSubscriberSearchIndexAsync(config.SubscriberSearchIndexId);
                    return new SearchIndexHandler(subscription, config.Id, searchIndex);
                default:
                    throw new ArgumentException("unsupported handler type");
            }
        }
    }
}
