using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers.Handlers;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{

    public class EventHandlerFactory: IEventHandlerFactory
    {
        public EventHandlerFactory(
            IBlockchainProxyService blockchainProxy, 
            IEventProcessingConfigurationRepository configRepo, 
            ISubscriberQueueFactory subscriberQueueFactory = null,
            ISubscriberSearchIndexFactory subscriberSearchIndexFactory = null,
            ISubscriberStorageFactory subscriberRepositoryFactory = null)
            :this(
                 configRepo, 
                 configRepo, 
                 blockchainProxy,  
                 configRepo, 
                 blockchainProxy, 
                 subscriberQueueFactory,
                 subscriberSearchIndexFactory,
                 configRepo,
                 subscriberRepositoryFactory)
        {
        }

        public EventHandlerFactory(
            IEventSubscriptionStateRepository stateFactory, 
            IEventContractQueryConfigurationRepository contractQueryFactory = null,
            IContractQuery contractQueryHandler = null,
            IEventAggregatorConfigurationRepository eventAggregatorConfigurationFactory = null,
            IGetTransactionByHash getTransactionProxy = null,
            ISubscriberQueueFactory subscriberQueueFactory = null,
            ISubscriberSearchIndexFactory subscriberSearchIndexFactory = null,
            IEventRuleConfigurationRepository eventRuleConfigurationFactory = null,
            ISubscriberStorageFactory subscriberRepositoryFactory = null)
        {
            StateFactory = stateFactory;
            ContractQueryFactory = contractQueryFactory;
            ContractQueryHandler = contractQueryHandler;
            EventAggregatorConfigurationFactory = eventAggregatorConfigurationFactory;
            GetTransactionProxy = getTransactionProxy;
            SubscriberQueueFactory = subscriberQueueFactory;
            SubscriberSearchIndexFactory = subscriberSearchIndexFactory;
            EventRuleConfigurationFactory = eventRuleConfigurationFactory;
            SubscriberRepositoryFactory = subscriberRepositoryFactory;
        }

        public IEventSubscriptionStateRepository StateFactory { get; }
        public IEventContractQueryConfigurationRepository ContractQueryFactory { get; }
        public IContractQuery ContractQueryHandler { get; }
        public IEventAggregatorConfigurationRepository EventAggregatorConfigurationFactory { get; }
        public IGetTransactionByHash GetTransactionProxy { get; }
        public ISubscriberQueueFactory SubscriberQueueFactory { get; }
        public ISubscriberSearchIndexFactory SubscriberSearchIndexFactory { get; }
        public IEventRuleConfigurationRepository EventRuleConfigurationFactory { get; }
        public ISubscriberStorageFactory SubscriberRepositoryFactory { get; }

        public async Task<IEventHandler> LoadAsync(IEventSubscription subscription, IEventHandlerDto config)
        { 
            switch (config.HandlerType)
            {
                case EventHandlerType.Rule:
                    CheckDependency(EventRuleConfigurationFactory);
                    var ruleConfig = await EventRuleConfigurationFactory.GetEventRuleConfigurationAsync(config.Id);
                    return new EventRule(subscription, config.Id, ruleConfig);

                case EventHandlerType.Aggregate:
                    CheckDependency(EventAggregatorConfigurationFactory);
                    var aggregatorConfig = await EventAggregatorConfigurationFactory.GetEventAggregationConfigurationAsync(config.Id);
                    return new EventAggregator(subscription, config.Id, aggregatorConfig);

                case EventHandlerType.ContractQuery:
                    CheckDependency(ContractQueryFactory);
                    CheckDependency(ContractQueryHandler);
                    var queryConfig = await ContractQueryFactory.GetContractQueryConfigurationAsync(subscription.SubscriberId, config.Id);
                    return new ContractQueryEventHandler(subscription, config.Id, ContractQueryHandler, queryConfig);

                case EventHandlerType.Queue:
                    CheckDependency(SubscriberQueueFactory);
                    var queue = await SubscriberQueueFactory.GetSubscriberQueueAsync(subscription.SubscriberId, config.SubscriberQueueId);
                    return new QueueHandler(subscription, config.Id, queue);

                case EventHandlerType.GetTransaction:
                    CheckDependency(GetTransactionProxy);
                    return new GetTransactionEventHandler(subscription, config.Id, GetTransactionProxy);

                case EventHandlerType.Index:
                    CheckDependency(SubscriberSearchIndexFactory);
                    var searchIndex = await SubscriberSearchIndexFactory.GetSubscriberSearchIndexAsync(subscription.SubscriberId, config.SubscriberSearchIndexId);
                    return new SearchIndexHandler(subscription, config.Id, searchIndex);

                case EventHandlerType.Store:
                    CheckDependency(SubscriberRepositoryFactory);
                    var logRepository = await SubscriberRepositoryFactory.GetLogRepositoryAsync(subscription.SubscriberId, config.SubscriberRepositoryId);
                    return new StorageHandler(subscription, config.Id, logRepository);

                default:
                    throw new ArgumentException("unsupported handler type");
            }
        }

        private void CheckDependency<FactoryType>(FactoryType factory)
        {
            if(factory == null) throw new Exception($"EventHanderFactory error. Event handler dependency is null: '{typeof(FactoryType).Name}.'");
        }
    }
}
