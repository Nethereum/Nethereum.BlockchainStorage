using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public interface IEventProcessingConfigurationRepository: 
        IEventSubscriptionStateFactory, 
        IEventContractQueryConfigurationFactory, 
        IEventAggregatorConfigurationFactory,
        ISubscriberQueueConfigurationFactory,
        ISubscriberSearchIndexConfigurationFactory,
        IEventHandlerHistoryDb,
        IEventRuleConfigurationFactory,
        ISubscriberRepositoryConfigurationFactory
    {
        Task<SubscriberContractDto> GetContractAsync(long contractId);
        Task<EventSubscriptionAddressDto[]> GetEventSubscriptionAddressesAsync(long eventSubscriptionId);
        Task<EventSubscriptionDto[]> GetEventSubscriptionsAsync(long subscriberId);
        Task<ParameterConditionDto[]> GetParameterConditionsAsync(long eventSubscriptionId);
        Task<SubscriberDto[]> GetSubscribersAsync(long partitionId);
        Task<EventHandlerDto[]> GetEventHandlers(long eventSubscriptionId);

    }
}
