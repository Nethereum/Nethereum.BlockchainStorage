using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public interface IEventProcessingConfigurationRepository: 
        IEventSubscriptionStateRepository, 
        IEventContractQueryConfigurationRepository, 
        IEventAggregatorConfigurationRepository,
        ISubscriberQueueConfigurationRepository,
        ISubscriberSearchIndexConfigurationRepository,
        IEventHandlerHistoryRepository,
        IEventRuleConfigurationRepository,
        ISubscriberRepositoryConfigurationRepository
    {
        Task<ISubscriberDto[]> GetSubscribersAsync(long partitionId);
        Task<ISubscriberContractDto> GetSubscriberContractAsync(long subscriberId, long contractId);
        Task<IEventSubscriptionAddressDto[]> GetEventSubscriptionAddressesAsync(long eventSubscriptionId);
        Task<IEventSubscriptionDto[]> GetEventSubscriptionsAsync(long subscriberId);

        Task<IEventHandlerDto[]> GetEventHandlersAsync(long eventSubscriptionId);
        Task<IParameterConditionDto[]> GetParameterConditionsAsync(long eventSubscriptionId);
        
    }
}
