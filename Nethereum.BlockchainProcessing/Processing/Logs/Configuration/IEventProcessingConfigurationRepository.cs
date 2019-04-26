using Nethereum.BlockchainProcessing.Processing.Logs.Handling;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface IEventProcessingConfigurationRepository
    {
        ISubscriberRepository Subscribers { get; }
        ISubscriberStorageRepository SubscriberStorage { get; }
        ISubscriberContractRepository SubscriberContracts { get; }
        ISubscriberQueueRepository SubscriberQueues { get; }
        ISubscriberSearchIndexRepository SubscriberSearchIndexes { get; }
        IEventSubscriptionStateRepository EventSubscriptionStates { get;}
        IEventContractQueryConfigurationRepository EventContractQueries { get;}
        IEventHandlerHistory EventHandlerHistory { get;}
        IEventRuleRepository EventRules { get;}
        IEventAggregatorRepository EventAggregators { get;}
        IEventSubscriptionAddressRepository EventSubscriptionAddresses { get;}
        IEventSubscriptionRepository EventSubscriptions { get;}
        IEventHandlerRepository EventHandlers { get;}
        IParameterConditionRepository ParameterConditions { get;}
        IContractQueryRepository ContractQueries { get;}
        IContractQueryParameterRepository ContractQueryParameters { get;}
        IEventHandlerHistoryRepository EventHandlerHistoryRepo { get;}
    }
}
