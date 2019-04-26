using Nethereum.BlockchainProcessing.Processing.Logs.Handling;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{

    public class EventProcessingConfigurationRepository : IEventProcessingConfigurationRepository
    {

        public EventProcessingConfigurationRepository(
            ISubscriberRepository subscriberRepository,
            ISubscriberContractRepository subscriberContractRepository,
            IEventSubscriptionRepository eventSubscriptionRepository,
            IEventSubscriptionAddressRepository eventSubscriptionAddressRepository,
            IEventHandlerRepository eventHandlerRepository,
            IParameterConditionRepository parameterConditionRepository,
            IEventSubscriptionStateRepository eventSubscriptionStateRepository,
            IContractQueryRepository contractQueryRepository,
            IContractQueryParameterRepository contractQueryParameterRepository,
            IEventAggregatorRepository eventAggregatorRepository,
            ISubscriberQueueRepository subscriberQueueRepository,
            ISubscriberSearchIndexRepository subscriberSearchIndexRepository,
            IEventHandlerHistoryRepository eventHandlerHistoryRepository,
            IEventRuleRepository eventRuleRepository,
            ISubscriberStorageRepository subscriberStorageRepository
            )
        {
            Subscribers = subscriberRepository;
            SubscriberContracts = subscriberContractRepository;
            EventSubscriptions = eventSubscriptionRepository;
            EventSubscriptionAddresses = eventSubscriptionAddressRepository;
            EventHandlers = eventHandlerRepository;
            ParameterConditions = parameterConditionRepository;
            EventSubscriptionStates = eventSubscriptionStateRepository;
            ContractQueries = contractQueryRepository;
            ContractQueryParameters = contractQueryParameterRepository;
            EventAggregators = eventAggregatorRepository;
            SubscriberQueues = subscriberQueueRepository;
            SubscriberSearchIndexes = subscriberSearchIndexRepository;
            EventHandlerHistoryRepo = eventHandlerHistoryRepository;
            EventRules = eventRuleRepository;
            SubscriberStorage = subscriberStorageRepository;

            EventContractQueries = new EventContractQueryConfigurationRepository(ContractQueries, SubscriberContracts, ContractQueryParameters);
            EventHandlerHistory = new EventHandlerHistory(EventHandlerHistoryRepo);
        }

        public ISubscriberRepository Subscribers { get; }
        public ISubscriberContractRepository SubscriberContracts { get; }
        public ISubscriberQueueRepository SubscriberQueues { get; }
        public ISubscriberSearchIndexRepository SubscriberSearchIndexes { get; }

        public ISubscriberStorageRepository SubscriberStorage { get; }
        public IEventSubscriptionRepository EventSubscriptions { get; }
        public IEventSubscriptionAddressRepository EventSubscriptionAddresses { get; }
        public IEventHandlerRepository EventHandlers { get; }
        public IParameterConditionRepository ParameterConditions { get; }
        public IEventSubscriptionStateRepository EventSubscriptionStates { get; }
        public IContractQueryRepository ContractQueries { get; }
        public IContractQueryParameterRepository ContractQueryParameters { get; }
        public IEventAggregatorRepository EventAggregators { get; }

        public IEventHandlerHistoryRepository EventHandlerHistoryRepo { get; }
        public IEventRuleRepository EventRules { get; }

        public IEventContractQueryConfigurationRepository EventContractQueries {get; }

        public IEventHandlerHistory EventHandlerHistory {get; }

    }
}
