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
                 configRepo.EventSubscriptionStates, 
                 configRepo.EventContractQueries, 
                 blockchainProxy,  
                 configRepo.EventAggregators, 
                 blockchainProxy, 
                 configRepo.SubscriberQueues,
                 subscriberQueueFactory,
                 configRepo.SubscriberSearchIndexes,
                 subscriberSearchIndexFactory,
                 configRepo.EventRules,
                 configRepo.SubscriberStorage,
                 subscriberRepositoryFactory)
        {
        }

        public EventHandlerFactory(
            IEventSubscriptionStateRepository stateFactory, 
            IEventContractQueryConfigurationRepository contractQueryFactory = null,
            IContractQuery contractQueryHandler = null,
            IEventAggregatorRepository eventAggregatorRepository = null,
            IGetTransactionByHash getTransactionProxy = null,
            ISubscriberQueueRepository subscriberQueueRepository = null,
            ISubscriberQueueFactory subscriberQueueFactory = null,
            ISubscriberSearchIndexRepository subscriberSearchIndexRepository = null,
            ISubscriberSearchIndexFactory subscriberSearchIndexFactory = null,
            IEventRuleRepository eventRuleRepository = null,
            ISubscriberStorageRepository subscriberStorageRepository = null,
            ISubscriberStorageFactory subscriberRepositoryFactory = null)
        {
            StateFactory = stateFactory;
            ContractQueryFactory = contractQueryFactory;
            ContractQueryHandler = contractQueryHandler;
            EventAggregatorRepository = eventAggregatorRepository;
            GetTransactionProxy = getTransactionProxy;
            SubscriberQueueRepository = subscriberQueueRepository;
            SubscriberQueueFactory = subscriberQueueFactory;
            SubscriberSearchIndexRepository = subscriberSearchIndexRepository;
            SubscriberSearchIndexFactory = subscriberSearchIndexFactory;
            EventRuleRepository = eventRuleRepository;
            SubscriberStorageRepository = subscriberStorageRepository;
            SubscriberRepositoryFactory = subscriberRepositoryFactory;
        }

        public IEventSubscriptionStateRepository StateFactory { get; }
        public IEventContractQueryConfigurationRepository ContractQueryFactory { get; }
        public IContractQuery ContractQueryHandler { get; }
        public IEventAggregatorRepository EventAggregatorRepository { get; }
        public IGetTransactionByHash GetTransactionProxy { get; }
        public ISubscriberQueueRepository SubscriberQueueRepository { get; }
        public ISubscriberQueueFactory SubscriberQueueFactory { get; }
        public ISubscriberSearchIndexRepository SubscriberSearchIndexRepository { get; }
        public ISubscriberSearchIndexFactory SubscriberSearchIndexFactory { get; }
        public IEventRuleRepository EventRuleRepository { get; }
        public ISubscriberStorageRepository SubscriberStorageRepository { get; }
        public ISubscriberStorageFactory SubscriberRepositoryFactory { get; }

        public async Task<IEventHandler> LoadAsync(IEventSubscription subscription, IEventHandlerDto config)
        { 
            switch (config.HandlerType)
            {
                case EventHandlerType.Rule:
                    CheckDependency(EventRuleRepository);
                    var ruleConfig = await EventRuleRepository.GetAsync(config.Id).ConfigureAwait(false);
                    return new EventRule(subscription, config.Id, ruleConfig);

                case EventHandlerType.Aggregate:
                    CheckDependency(EventAggregatorRepository);
                    var aggregatorConfig = await EventAggregatorRepository.GetAsync(config.Id).ConfigureAwait(false);
                    return new EventAggregator(subscription, config.Id, aggregatorConfig);

                case EventHandlerType.ContractQuery:
                    CheckDependency(ContractQueryFactory);
                    CheckDependency(ContractQueryHandler);
                    var queryConfig = await ContractQueryFactory.GetContractQueryConfigurationAsync(subscription.SubscriberId, config.Id).ConfigureAwait(false);
                    return new ContractQueryEventHandler(subscription, config.Id, ContractQueryHandler, queryConfig);

                case EventHandlerType.Queue:
                    CheckDependency(SubscriberQueueFactory);
                    CheckDependency(SubscriberQueueRepository);
                    var queueConfig = await SubscriberQueueRepository.GetAsync(subscription.SubscriberId, config.SubscriberQueueId).ConfigureAwait(false);
                    var queue = await SubscriberQueueFactory.GetSubscriberQueueAsync(queueConfig).ConfigureAwait(false);
                    return new QueueHandler(subscription, config.Id, queue);

                case EventHandlerType.GetTransaction:
                    CheckDependency(GetTransactionProxy);
                    return new GetTransactionEventHandler(subscription, config.Id, GetTransactionProxy);

                case EventHandlerType.Index:
                    CheckDependency(SubscriberSearchIndexFactory);
                    CheckDependency(SubscriberSearchIndexRepository);
                    var indexConfig = await SubscriberSearchIndexRepository.GetAsync(subscription.SubscriberId, config.SubscriberSearchIndexId).ConfigureAwait(false);
                    var searchIndex = await SubscriberSearchIndexFactory.GetSubscriberSearchIndexAsync(indexConfig).ConfigureAwait(false);
                    return new SearchIndexHandler(subscription, config.Id, searchIndex);

                case EventHandlerType.Store:
                    CheckDependency(SubscriberRepositoryFactory);
                    var storageConfig = await SubscriberStorageRepository.GetAsync(subscription.SubscriberId, config.SubscriberRepositoryId).ConfigureAwait(false);
                    var logRepository = await SubscriberRepositoryFactory.GetLogRepositoryAsync(storageConfig).ConfigureAwait(false);
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
