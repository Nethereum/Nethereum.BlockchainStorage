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

        public async Task<IEventHandler> LoadAsync(EventHandlerDto config, EventSubscriptionStateDto state)
        { 
            switch (config.HandlerType)
            {
                case EventHandlerType.Rule:
                    var ruleConfig = await EventRuleConfigurationFactory.GetEventRuleConfigurationAsync(config.Id);
                    return new EventRule(config.Id, state, ruleConfig);
                case EventHandlerType.Aggregate:
                    var aggregatorConfig = await EventAggregatorConfigurationFactory.GetEventAggregationConfigurationAsync(config.Id);
                    return new EventAggregator(config.Id, state, aggregatorConfig);
                case EventHandlerType.ContractQuery:
                    var queryConfig = await ContractQueryFactory.GetContractQueryConfigurationAsync(config.Id);
                    return new ContractQueryEventHandler(config.Id, ContractQueryHandler, state, queryConfig);
                case EventHandlerType.Queue:
                    var queue = await SubscriberQueueFactory.GetSubscriberQueueAsync(config.SubscriberQueueId);
                    return new QueueHandler(config.Id, state, queue);
                case EventHandlerType.GetTransaction:
                    return new GetTransactionEventHandler(config.Id, state, GetTransactionProxy);
                case EventHandlerType.Index:
                    var searchIndex = await SubscriberSearchIndexFactory.GetSubscriberSearchIndexAsync(config.SubscriberSearchIndexId);
                    return new SearchIndexHandler(config.Id, state, searchIndex);
                default:
                    throw new ArgumentException("unsupported handler type");
            }
        }
    }
}
